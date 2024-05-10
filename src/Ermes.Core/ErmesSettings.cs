using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes
{
    public class ErmesSettings
    {
        public string CorsOrigins { get; set; }
        public bool HangfireEnabled { get; set; }
        public string ErmesProject { get; set; } = "FASTER";
        public string WebApp { get; set; }
    }
}
