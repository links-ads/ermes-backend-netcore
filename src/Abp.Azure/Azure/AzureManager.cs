using Abp.Azure.Configuration;
using Abp.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Azure
{
    public class AzureManager : IAzureManager
    {
        private readonly IAzureConnectionProvider _connectionProvider;

        public AzureManager(IAzureConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }


        public AzureStorageManager GetStorageManager(string containerName)
        {
            return new AzureStorageManager(_connectionProvider.GetStorageConnectionString(), containerName);
        }
    }
}
