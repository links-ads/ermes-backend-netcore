using Abp.Azure;
using Abp.UI;
using Ermes.Enums;
using Ermes.ExternalServices.Csi.Configuration;
using Ermes.Operations;
using Ermes.Reports;
using Ermes.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private static HttpClient CsiClientPresidi;
        private readonly OperationManager _operationManager;
        private const string SUBJECT_CODE = "FAS";
        private readonly IAzureManager _azureManager;

        public CsiManager(CsiConnectionProvider connectionProvider, OperationManager operationManager, IAzureManager azureManager)
        {
            _connectionProvider = connectionProvider;
            CsiClient = GetCsiClient();
            CsiClientPresidi = GetCsiClient(true);
            _operationManager = operationManager;
            _azureManager = azureManager;
        }

        private HttpClient GetCsiClient(bool presidi = false)
        {
            HttpClient client = new HttpClient();
            byte[] byteArray;
            if(presidi)
                byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _connectionProvider.GetUsername_Presidi(), _connectionProvider.GetPassword_Presidi()));
            else
                byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _connectionProvider.GetUsername(), _connectionProvider.GetPassword()));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(presidi ? _connectionProvider.GetBaseUrl_Presidi() : _connectionProvider.GetBaseUrl());
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

        public async Task<int> InsertInterventiVolontariAsync(long personId, int personLegaycId, double? latitude, double? longitude, string activity, DateTime timestamp, string status, int? operationId = null)
        {
            var builder = new UriBuilder(CsiClient.BaseAddress + "/insertInterventiVolontari");
            string requestBody = JsonConvert.SerializeObject(new Intervention()
            {
                subjectCode = SUBJECT_CODE,
                latitude = latitude?.ToString(),
                longitude = longitude?.ToString(),
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

        public async Task InserisciFromFaster(
                long personId,
                int personLegaycId,
                int reportId,
                string creator,
                double latitude,
                double longitude,
                string description,
                string hazard,
                string status,
                List<string> mediaURIs
        )
        {
            var builder = new UriBuilder(CsiClientPresidi.BaseAddress + "/inserisciFromFaster");


            //Do not want to store on local DB the fule byte array of the attachments;
            //localbody will be stored in localDB without byte[], while bodys represent the full request sent to CSI service
            InsertReport localBody = new InsertReport()
            {
                mittente = creator,
                latitudine = latitude,
                longitudine = longitude,
                descrizione = description,
                fenomenoLabelList = new string[] { hazard },
                statoSegnalazioneLabel = status,
            };

            InsertReport body = (InsertReport) localBody.Clone();

            if (mediaURIs != null && mediaURIs.Count > 0)
            {
                var _azureReportStorageManager = _azureManager.GetStorageManager(ResourceManager.GetBasePath(ResourceManager.Reports.ContainerName));
                foreach (var fileName in mediaURIs)
                {
                    string mediaPath = ResourceManager.Reports.GetMediaPath(reportId, fileName);
                    ReportAttachment ra = new ReportAttachment(fileName, mediaPath);
                    localBody.allegatiSegnalazione.Add(ra);
                    var file = await _azureReportStorageManager.GetFile(ResourceManager.Reports.GetRelativeMediaPath(reportId, fileName));
                    body.allegatiSegnalazione.Add(new ReportAttachment(fileName, mediaPath, file));
                }
            }

            string requestBody = JsonConvert.SerializeObject(body);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(builder.ToString()),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            var op = new Operation()
            {
                Request = JsonConvert.SerializeObject(localBody),
                Type = VolterOperationType.InsertReport,
                PersonId = personId,
                PersonLegacyId = personLegaycId
            };

            op.Id = await _operationManager.InsertOrUpdateOperationAsync(op);
            await SendRequestPresidiInternal(request, op);
        }

        private async Task<int> SendRequestInternal(HttpRequestMessage request, Operation op)
        {
            try
            {
                using CancellationTokenSource tokenSource = new CancellationTokenSource(4000);
                HttpResponseMessage response = await CsiClient.SendAsync(request, cancellationToken: tokenSource.Token);
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

                    var result = JsonConvert.DeserializeObject<VolterResponse>(responseValue);
                    //var result = new VolterResponse()
                    //{
                    //    DescriptionOutcome = "Elaborazione terminata correttamente",
                    //    ProcessedCode = "0001",
                    //    VolterId = 198061
                    //};
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

        private async Task<int> SendRequestPresidiInternal(HttpRequestMessage request, Operation op)
        {
            try
            {
                using CancellationTokenSource tokenSource = new CancellationTokenSource(4000);
                HttpResponseMessage response = await CsiClientPresidi.SendAsync(request, cancellationToken: tokenSource.Token);
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

                    var result = JsonConvert.DeserializeObject<PresidiResponse>(responseValue);

                    if (result.status == 200)
                    {
                        if (result.items != null)
                        {
                            op.OperationLegacyId = result.items.id;
                            op.PresidiResponse = result;
                            return op.OperationLegacyId;
                        }
                        else
                            return 0;
                    }
                    else
                        op.ErrorMessage = string.Format($"CSI Service InserisciFromFaster error {result.status}");
                }
                else
                    op.ErrorMessage = string.Format($"CSI Service InserisciFromFaster not available, HTTP status code: {response.StatusCode}");
            }
            catch (Exception e)
            {
                op.ErrorMessage = string.Format($"CSI Service InserisciFromFaster not available: {e.Message}");
            }

            return -1;
        }
    }
}
