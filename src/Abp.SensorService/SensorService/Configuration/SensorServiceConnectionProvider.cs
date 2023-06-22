using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Abp.SensorService.Configuration
{
    public class SensorServiceConnectionProvider: ISensorServiceConnectionProvider
    {
        private readonly IOptions<AbpSensorServiceSettings> _settings;

        public SensorServiceConnectionProvider(IOptions<AbpSensorServiceSettings> settings)
        {
            _settings = settings;
        }
        public string GetApiKey()
        {
            if (_settings == null || _settings.Value == null)
                throw new ConfigurationErrorsException("An api key is expected for sensor service module");

            return _settings.Value.ApiKey;
        }

        public string GetBasePath()
        {
            if (_settings == null || _settings.Value == null)
                throw new ConfigurationErrorsException("A base path is expected for sensor service module");

            return _settings.Value.BasePath;
        }
    }
}
