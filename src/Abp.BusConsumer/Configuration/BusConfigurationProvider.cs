using Microsoft.Extensions.Options;
using System;
using System.Configuration;

namespace Abp.BusConsumer.Configuration
{
    public class BusConfigurationProvider : IBusConfigurationProvider
    {
        private readonly IOptions<BusConsumerSettings> _busSettings;
        public BusConfigurationProvider(IOptions<BusConsumerSettings> busSettings)
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

        public bool IsEnabled()
        {
            if (_busSettings == null || _busSettings.Value == null)
                throw new ConfigurationErrorsException("Bus configuration errror");

            return _busSettings.Value.IsEnabled;
        }

        public string GetHostname()
        {
            if (_busSettings == null || _busSettings.Value == null || String.IsNullOrWhiteSpace(_busSettings.Value.Hostname))
                throw new ConfigurationErrorsException("A Hostname string is expected for Bus configuration");

            return _busSettings.Value.Hostname;
        }

        public string GetUsername()
        {
            if (_busSettings == null || _busSettings.Value == null || String.IsNullOrWhiteSpace(_busSettings.Value.Username))
                throw new ConfigurationErrorsException("A Username string is expected for Bus configuration");

            return _busSettings.Value.Username;
        }

        public string GetPassword()
        {
            if (_busSettings == null || _busSettings.Value == null || String.IsNullOrWhiteSpace(_busSettings.Value.Password))
                throw new ConfigurationErrorsException("A Password string is expected for Bus configuration");

            return _busSettings.Value.Password;
        }

        public int GetPort()
        {
            if (_busSettings == null || _busSettings.Value == null || _busSettings.Value.Port == 0)
                throw new ConfigurationErrorsException("A Port value is expected for Bus configuration");

            return _busSettings.Value.Port;
        }

        public string GetVirtualHost()
        {
            if (_busSettings == null || _busSettings.Value == null || String.IsNullOrWhiteSpace(_busSettings.Value.VirtualHost))
                throw new ConfigurationErrorsException("A VirtualHost string is expected for Bus configuration");

            return _busSettings.Value.VirtualHost;
        }

        public string GetExchange()
        {
            if (_busSettings == null || _busSettings.Value == null || String.IsNullOrWhiteSpace(_busSettings.Value.Exchange))
                throw new ConfigurationErrorsException("A Exchange string is expected for Bus configuration");

            return _busSettings.Value.Exchange;
        }

        public string GetQueue()
        {
            if (_busSettings == null || _busSettings.Value == null || String.IsNullOrWhiteSpace(_busSettings.Value.Queue))
                throw new ConfigurationErrorsException("A Queue string is expected for Bus configuration");

            return _busSettings.Value.Queue;
        }

        public bool MustDeclareBindings()
        {
            return _busSettings.Value.MustDeclareBindings;
        }
    }
}
