using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Abp.AzureCognitiveServices.CognitiveServices.Configuration
{
    public class ComputerVisionConnectionProvider : IComputerVisionConnectionProvider
    {
        private readonly IOptions<AbpAzureCognitiveServicesSettings> _cognitiveServiceSettings;
        public ComputerVisionConnectionProvider(IOptions<AbpAzureCognitiveServicesSettings> cognitiveServiceSettings)
        {
            _cognitiveServiceSettings = cognitiveServiceSettings;
        }
        public string GetEndpoint()
        {
            if (_cognitiveServiceSettings == null || _cognitiveServiceSettings.Value == null)
                throw new ConfigurationErrorsException("An Endpoint string is expected for Cognitive Services");

            return _cognitiveServiceSettings.Value.Endpoint;
        }

        public string GetSubscriptionKey()
        {
            if (_cognitiveServiceSettings == null || _cognitiveServiceSettings.Value == null)
                throw new ConfigurationErrorsException("An Endpoint string is expected for Cognitive Services");

            return _cognitiveServiceSettings.Value.SubscriptionKey;
        }
    }
}
