using Ermes.Dto.Datatable;
using Ermes.Reports.Dto;
using Ermes.Teams.Dto;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ermes.Tests.Services
{
    public class ReportTests: ErmesTestBase
    {
        public ReportTests()
        {

        }

        [Fact]
        public async Task Should_Get_Reports()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Reports/GetReports" + BASE_QUERY_PARAMS + "&Visibility=All");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<DTResult<ReportDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(4);

            token = await GetToken(USERNAME_OM_CHILD);
            token.ShouldNotBeNull();
            client = GetApiClient(token);
            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue2 = await SendHttpRequest(request2, client);
            responseValue2.ShouldNotBeNull();
            var apiOutput2 = JsonConvert.DeserializeObject<DTResult<ReportDto>>(responseValue2);
            apiOutput2.data.Count.ShouldBe(3);

            token = await GetToken(USERNAME_CITIZEN);
            token.ShouldNotBeNull();
            client = GetApiClient(token);
            HttpRequestMessage request3 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue3 = await SendHttpRequest(request3, client);
            responseValue3.ShouldNotBeNull();
            var apiOutput3 = JsonConvert.DeserializeObject<DTResult<ReportDto>>(responseValue3);
            apiOutput3.data.Count.ShouldBe(2);
        }

    }
}
