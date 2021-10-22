using Abp.CsiServices.Csi.Configuration;
using Abp.CsiServices.Csi.Dto;
using Abp.UI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Abp.CsiServices.Csi
{
    public class CsiManager : ICsiManager
    {
        private readonly ICsiConnectionProvider _connectionProvider;
        private static HttpClient CsiClient;
        private readonly ILogger Logger;
        private const string SUBJECT_CODE = "FAS";

        public CsiManager(ICsiConnectionProvider connectionProvider, ILogger<CsiManager> logger)
        {
            _connectionProvider = connectionProvider;
            CsiClient = GetCsiClient();
            Logger = logger;
        }

        private HttpClient GetCsiClient()
        {
            HttpClient client = new HttpClient();
            var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _connectionProvider.GetUsername(), _connectionProvider.GetPassword()));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(_connectionProvider.GetBaseUrl());
            return client;
        }

        public async Task<int> SearchVolontarioAsync(string taxCode)
        {
            //CSI service only accepts HTTP GET request with params in the body
            //there's no possibility to use query string params

            var builder = new UriBuilder(CsiClient.BaseAddress + "/SearchVolontario");
            string requestBody = JsonConvert.SerializeObject(new SearchVolontarioInput()
            {
                subjectCode = SUBJECT_CODE,
                fiscalCodeVoluntary = taxCode
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(builder.ToString()),
                Content =  new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            try
            {
                HttpResponseMessage response = await CsiClient.SendAsync(request);
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

                    var result = JsonConvert.DeserializeObject<SearchVolontarioOutput>(responseValue);
                    if (result.ProcessedCodeTypeEnum == SearchVolontarioOutput.ProcessedCodeType.ElaborazioneTerminataCorretamente)
                        return result.VolterId;
                    else
                        Logger.LogError($"################ CSI Service error {result.ProcessedCode}: {result.DescriptionOutcome}");
                }
                else
                    Logger.LogError($"################ CSI Service not available");
            }
            catch (Exception e)
            {
                Logger.LogError($"################ CSI Service not available");
                throw new UserFriendlyException("CSI service not available");
            }

            return -1;
        }
    }
}
