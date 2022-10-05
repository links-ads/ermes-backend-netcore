using Ermes.Dto.Datatable;
using Ermes.Roles.Dto;
using Ermes.Teams.Dto;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Ermes.Authorization.AppPermissions;

namespace Ermes.Tests.Services
{
    public class TeamTests : ErmesTestBase
    {
        public TeamTests()
        {

        }

        [Fact]
        public async Task Should_Retrieve_Teams()
        {
            var token = await GetToken(USERNAME_ADMIN, "shelter2020!");
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Teams/GetTeams" + BASE_QUERY_PARAMS);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<DTResult<TeamOutputDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(3);

            token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            client = GetApiClient(token);
            uri = new Uri(client.BaseAddress, "Teams/GetTeams" + BASE_QUERY_PARAMS);
            request = new HttpRequestMessage(HttpMethod.Get, uri);
            responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            apiOutput = JsonConvert.DeserializeObject<DTResult<TeamOutputDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(2);

            token = await GetToken(USERNAME_OM_CHILD);
            token.ShouldNotBeNull();
            client = GetApiClient(token);
            uri = new Uri(client.BaseAddress, "Teams/GetTeams" + BASE_QUERY_PARAMS);
            request = new HttpRequestMessage(HttpMethod.Get, uri);
            responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            apiOutput = JsonConvert.DeserializeObject<DTResult<TeamOutputDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(1);
        }


        //Get Team, add two member, check members, remove members
        [Fact]
        public async Task Should_Retrieve_Team()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Teams/GetTeamById?id=3");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<TeamOutputDto>(responseValue);
            apiOutput.Id.ShouldBe(3);
            apiOutput.Organization.ShouldNotBeNull();
            apiOutput.Members.Count.ShouldBe(0);

            var uri2 = new Uri(client.BaseAddress, "Teams/SetTeamMembers");
            var request2 = new HttpRequestMessage(HttpMethod.Post, uri2);
            var jsonContent = JsonConvert.SerializeObject(new
            {
                teamId = apiOutput.Id,
                membersGuids = new string[] { "0d20ab05-9cf4-468e-a4e5-f9e8424f7181", "93661dee-3d6f-47df-ae5d-739d24f1ad89" }
            });
            request2.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var responseValue2 = await SendHttpRequest(request2, client);
            var apiOutput2 = JsonConvert.DeserializeObject<bool>(responseValue2);
            apiOutput2.ShouldBeTrue();

            HttpRequestMessage request3 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue3 = await SendHttpRequest(request3, client);
            responseValue3.ShouldNotBeNull();
            var apiOutput3 = JsonConvert.DeserializeObject<TeamOutputDto>(responseValue3);
            apiOutput3.Id.ShouldBe(3);
            apiOutput3.Organization.ShouldNotBeNull();
            apiOutput3.Members.Count.ShouldBe(2);

            var uri4 = new Uri(client.BaseAddress, "Teams/SetTeamMembers");
            var request4 = new HttpRequestMessage(HttpMethod.Post, uri4);
            var jsonContent2 = JsonConvert.SerializeObject(new
            {
                teamId = apiOutput.Id,
                membersGuids = new string[] {}
            });
            request4.Content = new StringContent(jsonContent2, Encoding.UTF8, "application/json");
            var responseValue4 = await SendHttpRequest(request4, client);
            var apiOutput4 = JsonConvert.DeserializeObject<bool>(responseValue4);
            apiOutput4.ShouldBeTrue();

            HttpRequestMessage request5 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue5 = await SendHttpRequest(request5, client);
            responseValue5.ShouldNotBeNull();
            var apiOutput5 = JsonConvert.DeserializeObject<TeamOutputDto>(responseValue5);
            apiOutput5.Id.ShouldBe(3);
            apiOutput5.Organization.ShouldNotBeNull();
            apiOutput5.Members.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Delete_Team()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Teams/CreateOrUpdateTeam");
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var jsonContent = JsonConvert.SerializeObject(new
            {
                team = new {
                    id=0,
                    name="Team TMP",
                    organizationId=1
                }
            });
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var responseValue = await SendHttpRequest(request, client);
            int apiOutput = JsonConvert.DeserializeObject<int>(responseValue);
            apiOutput.ShouldBeGreaterThan(0);

            Uri uri2 = new Uri(client.BaseAddress, "Teams/DeleteTeam?id="+ apiOutput);
            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Delete, uri2);
            var responseValue2 = await SendHttpRequest(request2, client);
            responseValue2.ShouldNotBeNull();
            var apiOutput2 = JsonConvert.DeserializeObject<bool>(responseValue2);
            apiOutput2.ShouldBeTrue();
        }
    }
}
