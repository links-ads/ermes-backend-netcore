using Microsoft.Extensions.Options;
using System.Configuration;

namespace Abp.ErmesSocialNetCore.Social.Configuration
{
    public class SocialConnectionProvider : ISocialConnectionProvider
    {
        private readonly IOptions<AbpSocialSettings> _socialSettings;
        public SocialConnectionProvider(IOptions<AbpSocialSettings> socialSettings)
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
