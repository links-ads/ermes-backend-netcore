using Abp.Configuration.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp
{
    public static class AbpAzureConfigExtensions
    {
        public static AbpAzureSettings AbpAzureModule(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration
                .GetOrCreate("AbpAzureModule",
                    () => moduleConfigurations.AbpConfiguration.IocManager.Resolve<AbpAzureSettings>()
                );
        }
    }
}
