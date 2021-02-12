using Microsoft.Extensions.Options;
using System;
using System.Configuration;

namespace Abp.ErmesBusConsumer.Configuration
{
    public class ErmesBusConfigurationProvider : IErmesBusConfigurationProvider
    {
        private readonly IOptions<ErmesBusConsumerSettings> _busSettings;
        public ErmesBusConfigurationProvider(IOptions<ErmesBusConsumerSettings> busSettings)
        {
            _busSettings = busSettings;
        }
        
        public string GetConnectionString()
        {
            if (_busSettings == null || _busSettings.Value == null || String.IsNullOrWhiteSpace(_busSettings.Value.ConnectionString))
                throw new ConfigurationErrorsException("A Connection string is expected for Bus configuration");

            return _busSettings.Value.ConnectionString;
        }

        public string GetGroupId()
        {
            if (_busSettings == null || _busSettings.Value == null || String.IsNullOrWhiteSpace(_busSettings.Value.GroupId))
                throw new ConfigurationErrorsException("A GroupId string is expected for Bus configuration");

            return _busSettings.Value.GroupId;
        }

        public string[] GetTopicList()
        {
            if (_busSettings == null || _busSettings.Value == null ||_busSettings.Value.TopicList == null)
                throw new ConfigurationErrorsException("A Topics string is expected for Bus configuration");

            return _busSettings.Value.TopicList;
        }
    }
}
