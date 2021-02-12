using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.AzureCognitiveServices.CognitiveServices.Configuration
{
    public interface IComputerVisionConnectionProvider
    {
        string GetEndpoint();
        string GetSubscriptionKey();
    }
}
