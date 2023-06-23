using Abp.SensorService.Configuration;
using Abp.SensorService.Model;
using Abp.SensorService.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Abp.SensorService
{
    public class SensorServiceManager : ISensorServiceManager
    {
        private static HttpClient HttpClient;
        private const string ApiKeyHeaderName = "apikey";
        private readonly ISensorServiceConnectionProvider _connectionProvider;
        private const string HEADER_APPLICATION_JSON = "application/json";
        public SensorServiceManager(ISensorServiceConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            GetHttpClient();
        }

        private void GetHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add(ApiKeyHeaderName, _connectionProvider.GetApiKey());
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HEADER_APPLICATION_JSON));
            client.BaseAddress = new Uri(_connectionProvider.GetBasePath());

            HttpClient = client;
            return;
        }
        public async Task<SensorServiceSensor> GetMeasuresOfSensor(string stationId, string sensorId, DateTime dateStart, DateTime dateEnd)
        {
            if (HttpClient == null)
                GetHttpClient();
            Uri uri = new Uri(HttpClient.BaseAddress + string.Format("query/stations/{0}/sensor/{1}?dateStart={2}&dateEnd={3}", stationId, sensorId, string.Format("{0:yyyy-MM-ddTHH:mm:ss.fffZ}", dateStart), string.Format("{0:yyyy-MM-ddTHH:mm:ss.fffZ}", dateEnd)));
            var responseValue = await HttpHelper.SendHttpRequestAsync(HttpClient, HttpMethod.Get, uri);
            return JsonConvert.DeserializeObject<SensorServiceSensor>(responseValue);
        }


        public async Task<List<SensorServiceStation>> GetStations()
        {
            if (HttpClient == null)
                GetHttpClient();
            Uri uri = new Uri(HttpClient.BaseAddress + "query/stations");
            var jsonContent = JsonConvert.SerializeObject(new { });
            var responseValue = await HttpHelper.SendHttpRequestAsync(HttpClient, HttpMethod.Post, uri, jsonContent);
            return JsonConvert.DeserializeObject<List<SensorServiceStation>>(responseValue);
        }

        public async Task<SensorServiceStation> GetStationInfo(string stationId)
        {
            if (HttpClient == null)
                GetHttpClient();
            Uri uri = new Uri(HttpClient.BaseAddress + "query/stations/" + stationId);
            var responseValue = await HttpHelper.SendHttpRequestAsync(HttpClient, HttpMethod.Get, uri);
            return JsonConvert.DeserializeObject<SensorServiceStation>(responseValue);
        }

        public async Task<SensorServiceStation> GetStationSummary(string stationId, DateTime dateStart, DateTime dateEnd)
        {
            if (HttpClient == null)
                GetHttpClient();
            Uri uri = new Uri(HttpClient.BaseAddress + string.Format("query/stations/{0}/summary?dateStart={1}&dateEnd={2}", stationId, string.Format("{0:yyyy-MM-ddTHH:mm:ss.fffZ}", dateStart), string.Format("{0:yyyy-MM-ddTHH:mm:ss.fffZ}", dateEnd)));
            var responseValue = await HttpHelper.SendHttpRequestAsync(HttpClient, HttpMethod.Get, uri);
            return JsonConvert.DeserializeObject<SensorServiceStation>(responseValue);
        }

        public async Task<SensorServiceStation> CreateStation(string name, decimal latitude, decimal longitude, decimal altitude, string owner, string brand, string productCode = "", string address = "address")
        {
            if (HttpClient == null)
                GetHttpClient();

            Uri uri = new Uri(HttpClient.BaseAddress + "stations");
            var jsonContent = JsonConvert.SerializeObject(new
            {
                name,
                location = new decimal[] { latitude, longitude, altitude },
                address,
                owner,
                brand,
                productCode
            });
            var responseValue = await HttpHelper.SendHttpRequestAsync(HttpClient, HttpMethod.Post, uri, jsonContent);
            return JsonConvert.DeserializeObject<SensorServiceStation>(responseValue);
        }

        public async Task<SensorServiceSensor> CreateSensor(string stationId, string type, string description, string unit = "")
        {
            if (HttpClient == null)
                GetHttpClient();

            Uri uri = new Uri(string.Format("{0}stations/{1}/sensors", HttpClient.BaseAddress, stationId));
            var jsonContent = JsonConvert.SerializeObject(new
            {
                type,
                description,
                unit
            });
            var responseValue = await HttpHelper.SendHttpRequestAsync(HttpClient, HttpMethod.Post, uri, jsonContent);
            return JsonConvert.DeserializeObject<SensorServiceSensor>(responseValue);
        }

        public async Task<SensorServiceMeasure> CreateMeasure(string sensorId, DateTime dateStart, DateTime dateEnd, string measure, object metadata, string unit = "degree")
        {
            if (HttpClient == null)
                GetHttpClient();

            Uri uri = new Uri(string.Format("{0}sensors/{1}", HttpClient.BaseAddress, sensorId));
            var jsonContent = JsonConvert.SerializeObject(new
            {
                dateStart,
                dateEnd,
                measure,
                unit,
                metadata
            });
            var responseValue = await HttpHelper.SendHttpRequestAsync(HttpClient, HttpMethod.Post, uri, jsonContent);
            return JsonConvert.DeserializeObject<SensorServiceMeasure>(responseValue);
        }
    }
}
