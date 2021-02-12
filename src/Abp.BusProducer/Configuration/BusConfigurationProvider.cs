using Microsoft.Extensions.Options;
using System;
using System.Configuration;

namespace Abp.BusProducer.Configuration
{
    public class BusConfigurationProvider : IBusConfigurationProvider
    {
        private readonly IOptions<BusProducerSettings> _busSettings;
        public BusConfigurationProvider(IOptions<BusProducerSettings> busSettings)
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
