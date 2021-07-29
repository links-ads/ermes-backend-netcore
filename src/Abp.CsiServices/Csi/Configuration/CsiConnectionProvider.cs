using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Abp.CsiServices.Csi.Configuration
{
    public class CsiConnectionProvider : ICsiConnectionProvider
    {
        private readonly IOptions<AbpCsiSettings> _csiSettings;

        public CsiConnectionProvider(IOptions<AbpCsiSettings> csiSettings)
        {
            _csiSettings = csiSettings;
        }

        public string GetBaseUrl()
        {
            if (_csiSettings == null || _csiSettings.Value == null)
                throw new ConfigurationErrorsException("A base Url is expected for CSI service");

            return _csiSettings.Value.BaseUrl;
        }

        public string GetPassword()
        {
            if (_csiSettings == null || _csiSettings.Value == null)
                throw new ConfigurationErrorsException("A password is expected for CSI service");

            return _csiSettings.Value.Password;
        }

        public string GetUsername()
        {
            if (_csiSettings == null || _csiSettings.Value == null)
                throw new ConfigurationErrorsException("A username is expected for CSI service");

            return _csiSettings.Value.Username;
        }
    }
}
