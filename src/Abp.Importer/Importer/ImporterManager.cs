using Abp.Importer.Api;
using Abp.Importer.Client;
using Abp.Importer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Abp.Importer
{
    public class ImporterManager : IImporterManager
    {
        private readonly IImporterConnectionProvider _connectionProvider;

        public ImporterManager(IImporterConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            Configure();
        }

        private void Configure()
        {
            var apiKeyDict = new Dictionary<string, string>
            {
                { _connectionProvider.GetHeaderName(), _connectionProvider.GetApiKey() }
            };
            GlobalConfiguration.Instance = new GlobalConfiguration(new Dictionary<string, string>(), apiKeyDict, new Dictionary<string, string>(), _connectionProvider.GetBaseUrl());
        }

        public async Task<object> GetLayers(List<string> datatype_ids, string bbox, DateTime start, DateTime end, List<string> request_codes, bool includeMapRequests = false)
        {
            var api = new DashboardApi();
            return await api.GetLayersLayersGetAsync(datatype_ids, bbox, start, end, request_codes, includeMapRequests);
        }

        public async Task<object> GetTimeSeries(string datatype_id, string point, string crs, string request_code = null, string layer_name=null, DateTime? start = null, DateTime? end = null)
        {
            var api = new DashboardApi();
            return await api.GetTimeseriesTimeseriesGetAsync(datatype_id, point, crs, request_code, layer_name, start, end);
        }

        public async Task<object> GetMetadata(string metadata_id)
        {
            var api = new DatalakeUtilsApi();
            return await api.GetMetadataMetadataGetAsync(metadata_id);
        }

        public async Task<List<string>> DeleteMapRequests(List<string> mapRequestCodes)
        {
            var api = new DatalakeUtilsApi();
            var result = new List<string>();
            var deletedLayers = await api.DeleteLayersDeleteLayerDeleteAsync(null, mapRequestCodes);
            if(deletedLayers != null && deletedLayers.Count > 0)
                result = deletedLayers.Select(a => a.RequestCode).Distinct().ToList();
            return result;
        }

        public async Task<object> GetFilename(string layerName, string resourceId)
        {
            var api = new DownloadApi();
            return await api.GetResourcePathResourcePathGetAsync(layerName, resourceId);
        }
    }
}
