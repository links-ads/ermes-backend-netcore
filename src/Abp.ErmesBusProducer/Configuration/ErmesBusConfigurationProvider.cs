using Microsoft.Extensions.Options;
using System;
using System.Configuration;

namespace Abp.ErmesBusProducer.Configuration
{
    public class ErmesBusConfigurationProvider : IErmesBusConfigurationProvider
    {
        private readonly IOptions<ErmesBusProducerSettings> _busSettings;
        public ErmesBusConfigurationProvider(IOptions<ErmesBusProducerSettings> busSettings)
        {
            _busSettings = busSettings;
        }
        
        public string GetConnectionString()
        {
            if (_busSettings == null || _busSettings.Value == null || String.IsNullOrWhiteSpace(_busSettings.Value.ConnectionString))
                throw new ConfigurationErrorsException("A Connection string is expected for Bus configuration");

            return _busSettings.Value.ConnectionString;
        }
    }
}
