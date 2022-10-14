using Ermes.Dto.Datatable;
using Ermes.Missions.Dto;
using Ermes.Reports.Dto;
using Ermes.Tests.Geometry;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ermes.Tests.Services
{
    public class MissionTests: ErmesTestBase
    {
        private const string TYPE_MISSION = "Mission";
        public MissionTests()
        {

        }

        [Fact]
        public async Task Should_Get_Missions()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Missions/GetMissions" + BASE_QUERY_PARAMS);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<DTResult<MissionDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(4);

            token = await GetToken(USERNAME_OM_2);
            token.ShouldNotBeNull();
            client = GetApiClient(token);
            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue2 = await SendHttpRequest(request2, client);
            responseValue2.ShouldNotBeNull();
            var apiOutput2 = JsonConvert.DeserializeObject<DTResult<MissionDto>>(responseValue2);
            apiOutput2.data.Count.ShouldBe(1);

            token = await GetToken(USERNAME_OM_CHILD);
            token.ShouldNotBeNull();
            client = GetApiClient(token);
            HttpRequestMessage request3 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue3 = await SendHttpRequest(request3, client);
            responseValue3.ShouldNotBeNull();
            var apiOutput3 = JsonConvert.DeserializeObject<DTResult<MissionDto>>(responseValue3);
            apiOutput3.data.Count.ShouldBe(1);

            token = await GetToken(USERNAME_CITIZEN);
            token.ShouldNotBeNull();
            client = GetApiClient(token);
            HttpRequestMessage request4 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue4 = await SendHttpRequest(request4, client);
            responseValue4.ShouldNotBeNull();
            var apiOutput4 = JsonConvert.DeserializeObject<DTResult<MissionDto>>(responseValue4);
            apiOutput4.data.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_Get_Missions_With_Bbox()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Missions/GetMissions" + BASE_QUERY_PARAMS + BBOX_QUERY_RAVENNA);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<DTResult<MissionDto>>(responseValue);
            apiOutput.data.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Should_Compare_With_GeoJson_Created()
        {
            var token = await GetToken(USERNAME_OM);
            token.ShouldNotBeNull();
            var client = GetApiClient(token);
            Uri uri = new Uri(client.BaseAddress, "Missions/GetMissions" + BASE_QUERY_PARAMS + "&status[0]=Created");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue = await SendHttpRequest(request, client);
            responseValue.ShouldNotBeNull();
            var apiOutput = JsonConvert.DeserializeObject<DTResult<ReportDto>>(responseValue);
            int itemsCount = apiOutput.data.Count;
            itemsCount.ShouldBe(1);

            uri = new Uri(client.BaseAddress, "GeoJson/GetFeatureCollection" + BASE_QUERY_PARAMS + "&EntityTypes[0]=Mission&missionStatusTypes[0]=Created");
            HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Get, uri);
            var responseValue2 = await SendHttpRequest(request2, client);
            responseValue2.ShouldNotBeNull();

            var apiOutput2 = JsonConvert.DeserializeObject<Test_FeatureCollectionBase>(responseValue2);
            apiOutput2.Features.Where(f => f.Properties.Type == TYPE_MISSION).ToList().Count.ShouldBe(itemsCount);
        }
    }
}
