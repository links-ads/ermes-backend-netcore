using Abp.Importer.Configuration;
using Abp.Importer.Dto;
using Abp.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Importer
{
    public class ImporterManager : IImporterManager
    {
        private static HttpClient ImporterClient;
        private readonly IImporterConnectionProvider _connectionProvider;

        public ImporterManager(IImporterConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            GetImporterClient();
        }

        private void GetImporterClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add(_connectionProvider.GetHeaderName(), _connectionProvider.GetApiKey());
            client.BaseAddress = new Uri(_connectionProvider.GetBaseUrl());
            ImporterClient = client;
            return;
        }

        public async Task<List<string>> GetLayers()
        {
            if (ImporterClient == null)
                GetImporterClient();

            Uri uri = new Uri(ImporterClient.BaseAddress + "/layers");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpResponseMessage response = null;
            try
            {
                response = await ImporterClient.SendAsync(request);
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

                return JsonConvert.DeserializeObject<List<string>>(responseValue);
            }
            else
                throw new UserFriendlyException("ImporterNotAvailable");
        }
    }
}
