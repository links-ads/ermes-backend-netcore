using Abp.UI;
using Ermes.Dto.Datatable;
using Ermes.Organizations.Dto;
using Ermes.Reports.Dto;
using Ermes.Roles.Dto;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ermes.Tests.Services
{
    public class BackofficeTests: ErmesTestBase
    {
        public BackofficeTests()
        {

        }

        [Fact]
        public async Task Should_Retrieve_Roles()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Roles/GetRoles");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<GetRolesOutput>(responseValue);
            apiOutput.Roles.Count.ShouldBe(6);
        }

        //Admin should see 3 organizations, father om 2 and child om 1
        [Fact]
        public async Task Should_Retrieve_Organizations_Backoffice()
        {
            var token = await GetToken(USERNAME_ADMIN, "shelter2020!");
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Organizations/GetOrganizations" + BASE_QUERY_PARAMS);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<DTResult<OrganizationDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(3);

            token = await GetToken(USERNAME_OM);
            client = GetApiClient(token);
            uri = new Uri(client.BaseAddress, "Organizations/GetOrganizations" + BASE_QUERY_PARAMS);
            request = new HttpRequestMessage(HttpMethod.Get, uri);
            responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            apiOutput = JsonConvert.DeserializeObject<DTResult<OrganizationDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(2);

            token = await GetToken(USERNAME_OM_CHILD);
            token.ShouldNotBeNull();
            client = GetApiClient(token);
            uri = new Uri(client.BaseAddress, "Organizations/GetOrganizations" + BASE_QUERY_PARAMS);
            request = new HttpRequestMessage(HttpMethod.Get, uri);
            responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            apiOutput = JsonConvert.DeserializeObject<DTResult<OrganizationDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(1);
        }

        
        [Fact]
        public async Task Should_Retrieve_Organizations_Public()
        {
            var token = "fake";
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Profile/GetOrganizations" + BASE_QUERY_PARAMS);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<DTResult<OrganizationDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(2);

            client = GetApiClient(token);
            uri = new Uri(client.BaseAddress, "Profile/GetOrganizations" + BASE_QUERY_PARAMS + "&parentId=2");
            request = new HttpRequestMessage(HttpMethod.Get, uri);
            responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            apiOutput = JsonConvert.DeserializeObject<DTResult<OrganizationDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(0);

            client = GetApiClient(token);
            uri = new Uri(client.BaseAddress, "Profile/GetOrganizations" + BASE_QUERY_PARAMS + "&parentId=1");
            request = new HttpRequestMessage(HttpMethod.Get, uri);
            responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            apiOutput = JsonConvert.DeserializeObject<DTResult<OrganizationDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(1);
        }
    }
}
