using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.SocialMedia.Configuration
{
    public interface ISocialMediaConnectionProvider
    {
        string GetApiSecret();
        string GetApiKey();
        string GetBasePath();
    }
}
