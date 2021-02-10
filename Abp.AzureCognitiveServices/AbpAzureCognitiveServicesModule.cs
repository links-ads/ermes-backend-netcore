using Abp.AzureCognitiveServices.CognitiveServices;
using Abp.AzureCognitiveServices.CognitiveServices.ComputerVision;
using Abp.AzureCognitiveServices.CognitiveServices.Configuration;
using Abp.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Abp.AzureCognitiveServices
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpAzureCognitiveServicesModule : AbpModule
    {

        public override void PreInitialize()
        {
            IocManager.Register<AbpAzureCognitiveServicesSettings, AbpAzureCognitiveServicesSettings>();
            IocManager.Register<IComputerVisionConnectionProvider, ComputerVisionConnectionProvider>();
            IocManager.Register<IComputerVisionManager, ComputerVisionManager>();
            IocManager.Register<ICognitiveServicesManager, CognitiveServicesManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
