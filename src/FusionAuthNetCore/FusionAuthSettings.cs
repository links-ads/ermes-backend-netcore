using System;
using System.Collections.Generic;
using System.Text;

namespace FusionAuthNetCore
{
    public class FusionAuthSettings
    {
        public string ApiKey { get; set; }
        public string Url { get; set; }
        public string Tenant { get; set; }
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
