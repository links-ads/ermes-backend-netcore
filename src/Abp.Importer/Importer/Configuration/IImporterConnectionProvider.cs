using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.Importer.Configuration
{
    public interface IImporterConnectionProvider
    {
        string GetBaseUrl();
        string GetApiKey();
        string GetHeaderName();
    }
}
