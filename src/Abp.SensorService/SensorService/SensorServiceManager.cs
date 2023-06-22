using Abp.SensorService.Configuration;
using Abp.SensorService.Model;
using Abp.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        public Task GetMeasuresOfSensor(string stationId, string sensorId)
        {
            throw new NotImplementedException();
        }


        public async Task<List<SensorServiceStation>> GetStations()
        {
            if (HttpClient == null)
                GetHttpClient();

            Uri uri = new Uri(HttpClient.BaseAddress + "query/stations");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            var jsonContent = JsonConvert.SerializeObject(new
            {
            });
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            var responseValue = string.Empty;
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                {
                    var stream = t.Result;
                    using var reader = new StreamReader(stream);
                    responseValue = reader.ReadToEnd();
                });

                task.Wait();

                return JsonConvert.DeserializeObject<List<SensorServiceStation>>(responseValue);
            }
            else
                throw new UserFriendlyException("SensorServiceNotAvailable");
        }

        public async Task<SensorServiceStation> GetStationInfo(string stationId)
        {
            if (HttpClient == null)
                GetHttpClient();

            Uri uri = new Uri(HttpClient.BaseAddress + "query/stations/" + stationId);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            var responseValue = string.Empty;
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                {
                    var stream = t.Result;
                    using var reader = new StreamReader(stream);
                    responseValue = reader.ReadToEnd();
                });

                task.Wait();

                return JsonConvert.DeserializeObject<SensorServiceStation>(responseValue);
            }
            else
                throw new UserFriendlyException("SensorServiceNotAvailable");
        }

        public async Task<SensorServiceStation> CreateStation(string name, decimal latitude, decimal longitude, decimal altitude, string address = "address")
        {
            if (HttpClient == null)
                GetHttpClient();

            Uri uri = new Uri(HttpClient.BaseAddress + "stations");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            var jsonContent = JsonConvert.SerializeObject(new
            {
                name,
                location = new decimal[] {latitude, longitude, altitude},
                address
            });
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            var responseValue = string.Empty;
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                {
                    var stream = t.Result;
                    using var reader = new StreamReader(stream);
                    responseValue = reader.ReadToEnd();
                });

                task.Wait();

                return JsonConvert.DeserializeObject<SensorServiceStation>(responseValue);
            }
            else
                throw new UserFriendlyException("SensorServiceNotAvailable");
        }

        public async Task<SensorServiceSensor> CreateSensor(string stationId, string type, string description, string unit = "")
        {
            if (HttpClient == null)
                GetHttpClient();

            Uri uri = new Uri(string.Format("{0}stations/{1}/sensors", HttpClient.BaseAddress, stationId));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            var jsonContent = JsonConvert.SerializeObject(new
            {
                type,
                description,
                unit
            });
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            var responseValue = string.Empty;
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                {
                    var stream = t.Result;
                    using var reader = new StreamReader(stream);
                    responseValue = reader.ReadToEnd();
                });

                task.Wait();

                return JsonConvert.DeserializeObject<SensorServiceSensor>(responseValue);
            }
            else
                throw new UserFriendlyException("SensorServiceNotAvailable");
        }

        public async Task<SensorServiceMeasure> CreateMeasure(string sensorId, DateTime dateStart, DateTime dateEnd, string measure, object metadata, string unit = "degree")
        {
            if (HttpClient == null)
                GetHttpClient();

            Uri uri = new Uri(string.Format("{0}sensors/{1}", HttpClient.BaseAddress, sensorId));
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            var jsonContent = JsonConvert.SerializeObject(new
            {
                dateStart,
                dateEnd,
                measure,
                unit,
                metadata
            });
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            var responseValue = string.Empty;
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                {
                    var stream = t.Result;
                    using var reader = new StreamReader(stream);
                    responseValue = reader.ReadToEnd();
                });

                task.Wait();

                return JsonConvert.DeserializeObject<SensorServiceMeasure>(responseValue);
            }
            else
                throw new UserFriendlyException("SensorServiceNotAvailable");
        }
    }
}
