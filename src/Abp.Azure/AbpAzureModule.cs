using Abp.Azure.Configuration;
using Abp.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Azure
{
    /// <summary>
    /// This module to use Azure services (storage, etc...) with abp
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpAzureModule : AbpModule
    {

        public override void PreInitialize()
        {
            IocManager.Register<AbpAzureSettings, AbpAzureSettings>();
            IocManager.Register<IAzureConnectionProvider, AzureConnectionProvider>();
            IocManager.Register<IAzureManager, AzureManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
