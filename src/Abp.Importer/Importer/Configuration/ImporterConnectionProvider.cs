using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Abp.Importer.Configuration
{
    public class ImporterConnectionProvider : IImporterConnectionProvider
    {
        private readonly IOptions<AbpImporterSettings> _importerSettings;
        public ImporterConnectionProvider(IOptions<AbpImporterSettings> importerSettings)
        {
            _importerSettings = importerSettings;
        }

        public string GetApiKey()
        {
            if (_importerSettings == null || _importerSettings.Value == null)
                throw new ConfigurationErrorsException("An api key is expected for importer service");

            return _importerSettings.Value.ApiKey;
        }

        public string GetBaseUrl()
        {
            if (_importerSettings == null || _importerSettings.Value == null)
                throw new ConfigurationErrorsException("A base url is expected for importer service");

            return _importerSettings.Value.BaseUrl;
        }

        public string GetHeaderName()
        {
            if (_importerSettings == null || _importerSettings.Value == null)
                throw new ConfigurationErrorsException("An header name is expected for importer service");

            return _importerSettings.Value.HeaderName;
        }
    }
}
