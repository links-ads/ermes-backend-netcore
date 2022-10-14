using Ermes.Dto.Datatable;
using Ermes.Reports.Dto;
using Ermes.Tests.Geometry;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Ermes.Tests.Services
{
    public class ReportTests: ErmesTestBase
    {
        private const string TYPE_REPORT = "Report";
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

        [Fact]
        public async Task Should_Compare_With_GeoJson_All()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Reports/GetReports" + BASE_QUERY_PARAMS + "&visibility=All");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<DTResult<ReportDto>>(responseValue);
            int reportCount = apiOutput.data.Count;
            reportCount.ShouldBe(4);

            uri = new Uri(client.BaseAddress, "GeoJson/GetFeatureCollection" + BASE_QUERY_PARAMS + "&EntityTypes[0]=Report&reportVisibilityType=All");
            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue2 = await SendHttpRequest(request2, client);
            responseValue2.ShouldNotBeNull();

            var apiOutput2 = JsonConvert.DeserializeObject<Test_FeatureCollectionBase>(responseValue2);
            apiOutput2.Features.Where(f => f.Properties.Type == TYPE_REPORT).ToList().Count.ShouldBe(reportCount);
        }

        [Fact]
        public async Task Should_Compare_With_GeoJson_Private()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Reports/GetReports" + BASE_QUERY_PARAMS + "&Visibility=Private");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<DTResult<ReportDto>>(responseValue);
            int reportCount = apiOutput.data.Count;
            reportCount.ShouldBe(2);

            uri = new Uri(client.BaseAddress, "GeoJson/GetFeatureCollection" + BASE_QUERY_PARAMS + "&EntityTypes[0]=Report&reportVisibilityType=Private");
            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue2 = await SendHttpRequest(request2, client);
            responseValue2.ShouldNotBeNull();

            var apiOutput2 = JsonConvert.DeserializeObject<Test_FeatureCollectionBase>(responseValue2);
            apiOutput2.Features.Where(f => f.Properties.Type == TYPE_REPORT).ToList().Count.ShouldBe(reportCount);
        }

        [Fact]
        public async Task Should_Compare_With_GeoJson_Public()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Reports/GetReports" + BASE_QUERY_PARAMS + "&visibility=Public");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<DTResult<ReportDto>>(responseValue);
            int reportCount = apiOutput.data.Count;
            reportCount.ShouldBe(2);

            uri = new Uri(client.BaseAddress, "GeoJson/GetFeatureCollection" + BASE_QUERY_PARAMS + "&EntityTypes[0]=Report&reportVisibilityType=Public");
            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue2 = await SendHttpRequest(request2, client);
            responseValue2.ShouldNotBeNull();

            var apiOutput2 = JsonConvert.DeserializeObject<Test_FeatureCollectionBase>(responseValue2);
            apiOutput2.Features.Where(f => f.Properties.Type == TYPE_REPORT).ToList().Count.ShouldBe(reportCount);
        }
    }
}
