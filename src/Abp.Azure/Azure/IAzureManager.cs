using Abp.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.Azure
{
    public interface IAzureManager
    {
        AzureStorageManager GetStorageManager(string containerName);
    }
}
