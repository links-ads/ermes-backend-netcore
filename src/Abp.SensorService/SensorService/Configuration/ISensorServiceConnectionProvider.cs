using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.SensorService.Configuration
{
    public interface ISensorServiceConnectionProvider
    {
        string GetBasePath();
        string GetApiKey();
    }
}
