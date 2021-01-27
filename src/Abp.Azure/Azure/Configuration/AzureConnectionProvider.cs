using Microsoft.Extensions.Options;
using System.Configuration;

namespace Abp.Azure.Configuration
{
    public class AzureConnectionProvider : IAzureConnectionProvider
    {
        private readonly IOptions<AbpAzureSettings> _azureSettings;

        public AzureConnectionProvider(IOptions<AbpAzureSettings> azureSettings)
        {
            _azureSettings = azureSettings;
        }
        public string GetStorageConnectionString()
        {
            if (_azureSettings == null || _azureSettings.Value == null)
                throw new ConfigurationErrorsException("A connection string is expected for blobstorage service");

            return _azureSettings.Value.BlobStorageConnectionString;
        }

        public string GetStorageBasePath()
        {
            if (_azureSettings == null || _azureSettings.Value == null)
                throw new ConfigurationErrorsException("A connection string is expected for blobstorage service");

            return _azureSettings.Value.BlobStorageBasePath;
        }

    }
}
