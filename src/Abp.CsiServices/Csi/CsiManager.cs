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
            client.BaseAddress = new Uri(_connectionProvider.GetBaseUrl());
            return client;
        }

        public async Task<int> SearchVolontarioAsync(string taxCode)
        {
            var builder = new UriBuilder(CsiClient.BaseAddress + "/searchvolontario");
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["CodiceMateria"] = "1";
            query["CodFiscaleVolontario"] = taxCode;
            builder.Query = query.ToString();
            string url = builder.ToString();

            return 5;
            //HttpResponseMessage response = await CsiClient.GetAsync(url);
            //var responseValue = string.Empty;
            //if (response != null && response.StatusCode == HttpStatusCode.OK)
            //{
            //    Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
            //    {
            //        var stream = t.Result;
            //        using var reader = new StreamReader(stream);
            //        responseValue = reader.ReadToEnd();
            //    });

            //    task.Wait();

            //    var result = JsonConvert.DeserializeObject<SearchVolontarioOutput>(responseValue);
            //    if (result.EsiteElaborazione == SearchVolontarioOutput.EsitoElaborazioneType.ElaborazioneTerminataCorretamente)
            //        return result.codEsitoElaborazione;
            //    else
            //    {
            //        Logger.LogError($"################CSI Service error: {result.codEsitoElaborazione}");
            //        return -1;

            //    }
            //}
            //else
            //    throw new UserFriendlyException("CsiNotAvailable");
        }
    }
}
