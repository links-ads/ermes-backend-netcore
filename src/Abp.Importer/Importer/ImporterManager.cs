﻿using Abp.Importer.Api;
using Abp.Importer.Client;
using Abp.Importer.Configuration;
using System;
using System.Collections.Generic;
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

        public async Task<object> GetLayers(List<string> datatype_ids, string bbox, DateTime start, DateTime end)
        {
            var api = new DashboardApi();
            return await api.GetLayersLayersGetAsync(datatype_ids, bbox, start, end);
        }
    }
}