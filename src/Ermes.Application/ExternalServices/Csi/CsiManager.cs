using Abp.UI;
using Ermes.Enums;
using Ermes.ExternalServices.Csi.Configuration;
using Ermes.Operations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ermes.ExternalServices.Csi
{
    public class CsiManager : ErmesDomainServiceBase, ICsiManager
    {
        private readonly CsiConnectionProvider _connectionProvider;
        private static HttpClient CsiClient;
        private readonly OperationManager _operationManager;
        private const string SUBJECT_CODE = "FAS";

        public CsiManager(CsiConnectionProvider connectionProvider, OperationManager operationManager)
        {
            _connectionProvider = connectionProvider;
            CsiClient = GetCsiClient();
            _operationManager = operationManager;
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

        public async Task<int> SearchVolontarioAsync(string taxCode, long personId)
        {
            //CSI service only accepts HTTP GET request with params in the body
            //there's no possibility to use query-string params

            var builder = new UriBuilder(CsiClient.BaseAddress + "/SearchVolontario");
            string requestBody = JsonConvert.SerializeObject(new Registration()
            {
                subjectCode = SUBJECT_CODE,
                fiscalCodeVoluntary = taxCode
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(builder.ToString()),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            var op = new Operation()
            {
                Request = requestBody,
                Type = VolterOperationType.Registration,
                PersonId = personId
            };

            op.Id = await _operationManager.InsertOrUpdateOperationAsync(op);

            return await SendRequestInternal(request, op);
        }

        public async Task<int> InsertInterventiVolontariAsync(long personId, int personLegaycId, double latitude, double longitude, string activity, DateTime timestamp, string status, int? operationId = null)
        {
            var builder = new UriBuilder(CsiClient.BaseAddress + "/insertInterventiVolontari");
            string requestBody = JsonConvert.SerializeObject(new Intervention()
            {
                subjectCode = SUBJECT_CODE,
                latitude = latitude.ToString(),
                longitude = longitude.ToString(),
                missionDate = timestamp,
                status = status,
                voluntaryActivity = activity,
                volterID = personLegaycId.ToString(),
                operationId = status == AppConsts.CSI_OFFLINE && operationId.HasValue ? operationId.Value.ToString() : null
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(builder.ToString()),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            var op = new Operation()
            {
                Request = requestBody,
                Type = status == AppConsts.CSI_OFFLINE ? VolterOperationType.CloseIntervention : VolterOperationType.OpenIntervention,
                PersonId = personId,
                PersonLegacyId = personLegaycId 
            };

            op.Id = await _operationManager.InsertOrUpdateOperationAsync(op);

            return await SendRequestInternal(request, op);
        }

        private async Task<int> SendRequestInternal(HttpRequestMessage request, Operation op)
        {
            try
            {
                using CancellationTokenSource tokenSource = new CancellationTokenSource(4000);
                //HttpResponseMessage response = await CsiClient.SendAsync(request, cancellationToken: tokenSource.Token);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                var responseValue = string.Empty;
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    //Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                    //{
                    //    var stream = t.Result;
                    //    using var reader = new StreamReader(stream);
                    //    responseValue = reader.ReadToEnd();
                    //});

                    //task.Wait();

                    //var result = JsonConvert.DeserializeObject<VolterResponse>(responseValue);
                    var result = new VolterResponse()
                    {
                        DescriptionOutcome = "Elaborazione terminata correttamente",
                        ProcessedCode = "0001",
                        VolterId = 198061
                    };
                    if (result.ProcessedCodeTypeEnum == ProcessedCodeType.ElaborazioneTerminataCorretamente)
                    {
                        op.Response = result;
                        /*
                         * For SearchVolontario API, volterId contained in the response refers to volter internal person id
                         * For InsertInterventiVolontario API, volterId contained in the response refers to the volter internal operation id
                         * 
                         * This means that the same prop has different meaning, based on the called API service
                         */

                        if (op.Type == VolterOperationType.Registration)
                            op.PersonLegacyId = result.VolterId;
                        else
                            op.OperationLegacyId = result.VolterId;

                        return result.VolterId;
                    }
                    else
                        op.ErrorMessage = string.Format($"CSI Service error {result.ProcessedCode}: {result.DescriptionOutcome}");
                }
                else
                    op.ErrorMessage = string.Format($"CSI Service not available, HTTP status code: {response.StatusCode}");
            }
            catch (Exception e)
            {
                op.ErrorMessage = string.Format($"CSI Service not available: {e.Message}");
            }

            return -1;
        }
    }
}
