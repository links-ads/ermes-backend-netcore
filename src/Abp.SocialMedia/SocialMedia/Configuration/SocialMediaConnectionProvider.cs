using Microsoft.Extensions.Options;
using System.Configuration;

namespace Abp.SocialMedia.Configuration
{
    public class SocialMediaConnectionProvider : ISocialMediaConnectionProvider
    {
        private readonly IOptions<AbpSocialMediaSettings> _socialSettings;
        public SocialMediaConnectionProvider(IOptions<AbpSocialMediaSettings> socialSettings)
        {
            _socialSettings = socialSettings;
        }
        public string GetApiKey()
        {
            if (_socialSettings == null || _socialSettings.Value == null)
                throw new ConfigurationErrorsException("An api key is expected for Social service");

            return _socialSettings.Value.ApiKey;
        }

        public string GetApiSecret()
        {
            if (_socialSettings == null || _socialSettings.Value == null)
                throw new ConfigurationErrorsException("An api secret is expected for Social service");

            return _socialSettings.Value.ApiSecret;
        }

        public string GetBasePath()
        {
            if (_socialSettings == null || _socialSettings.Value == null)
                throw new ConfigurationErrorsException("An base path is expected for Social service");

            return _socialSettings.Value.BasePath;
        }
    }
}
