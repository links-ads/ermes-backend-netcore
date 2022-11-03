using Abp.Azure;
using Abp.UI;
using Ermes.Categories;
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
        private readonly CategoryManager _categoryManager;
        private const string SUBJECT_CODE = "FAS";
        private readonly IAzureManager _azureManager;
        private const string HEADER_APPLICATION_JSON = "application/json";

        public CsiManager(CsiConnectionProvider connectionProvider, OperationManager operationManager, IAzureManager azureManager, CategoryManager categoryManager)
        {
            _connectionProvider = connectionProvider;
            if(CsiClient == null)
                CsiClient = GetCsiClient();
            if(CsiClientPresidi == null)
                CsiClientPresidi = GetCsiClient(true);
            _operationManager = operationManager;
            _azureManager = azureManager;
            _categoryManager = categoryManager;
        }

        private HttpClient GetCsiClient(bool presidi = false)
        {
            HttpClient client;
            byte[] byteArray;
            if (presidi)
            {
                client = new HttpClient(new HttpClientHandler() { UseProxy = false });
                byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _connectionProvider.GetUsername_Presidi(), _connectionProvider.GetPassword_Presidi()));
            }
            else
            {
                client = new HttpClient();
                byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", _connectionProvider.GetUsername(), _connectionProvider.GetPassword()));
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(HEADER_APPLICATION_JSON));
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
                Content = new StringContent(requestBody, Encoding.UTF8, HEADER_APPLICATION_JSON)
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
                Content = new StringContent(requestBody, Encoding.UTF8, HEADER_APPLICATION_JSON)
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

        public async Task InserisciFromFaster(Report report)
        {
            var builder = new UriBuilder(CsiClientPresidi.BaseAddress + "/inserisciFromFaster");

            //Do not want to store on local DB the fule byte array of the attachments;
            //localbody will be stored in localDB without byte[], while body represents the full request sent to CSI service
            InsertReport localBody = new InsertReport()
            {
                mittente = report.Creator.Email,
                latitudine = report.Location.Y,
                longitudine = report.Location.X,
                descrizione = report.Description,
                fenomenoLabelList = new string[] { report.HazardString },
                statoSegnalazioneLabel = report.StatusString,
                notaList = new List<string>(),
                peoples = new List<ReportPeople>(),
                allegatiSegnalazione = new List<ReportAttachment>(),
                allegatiComunicazione = new List<ReportAttachment>(),
            };

            if(report.ExtensionData != null && report.ExtensionData.Count > 0)
            {
                var categories = await _categoryManager.GetCategoriesAsync();
                foreach (var item in report.ExtensionData)
                {
                    var category = categories.Single(c => c.Id == item.CategoryId);
                    //TODO: complete the switch with the mnagement of all types of categories, based on the destination API
                    switch (category.GroupKey)
                    {
                        case "People":
                            ReportPeople people = new ReportPeople(category.Translations.Where(t => t.Language == "it").Select(c => c.Name).FirstOrDefault(), int.Parse(item.Value));
                            localBody.peoples.Add(people);
                            break;
                        default:
                            break;
                    }
                }

            }

            InsertReport body = (InsertReport) localBody.Clone();

            if (report.MediaURIs != null && report.MediaURIs.Count > 0)
            {
                var _azureReportStorageManager = _azureManager.GetStorageManager(ResourceManager.GetBasePath(ResourceManager.Reports.ContainerName));
                foreach (var fileName in report.MediaURIs)
                {
                    string mediaPath = ResourceManager.Reports.GetMediaPath(report.Id, fileName);
                    ReportAttachment ra = new ReportAttachment(fileName, mediaPath);
                    localBody.allegatiSegnalazione.Add(ra);
                    var file = await _azureReportStorageManager.GetFile(ResourceManager.Reports.GetRelativeMediaPath(report.Id, fileName));
                    body.allegatiSegnalazione.Add(new ReportAttachment(fileName, mediaPath, file));
                }
            }

            string requestBody = JsonConvert.SerializeObject(body);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(builder.ToString()),
                Content = new StringContent(requestBody, Encoding.UTF8, HEADER_APPLICATION_JSON)
            };

            var op = new Operation()
            {
                Request = JsonConvert.SerializeObject(localBody),
                Type = VolterOperationType.InsertReport,
                PersonId = report.CreatorUserId.Value,
                PersonLegacyId = report.Creator.LegacyId ?? 0
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
