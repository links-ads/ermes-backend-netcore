using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.ExternalServices.Csi.Configuration
{
    public interface ICsiConnectionProvider
    {
        string GetUsername();
        string GetPassword();
        string GetBaseUrl();
    }
}
