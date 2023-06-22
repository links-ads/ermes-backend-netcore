using Abp.Modules;
using Abp.SensorService.Configuration;
using System;
using System.Reflection;

namespace Abp.SensorService
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpSensorServiceModule: AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpSensorServiceSettings, AbpSensorServiceSettings>();
            IocManager.Register<ISensorServiceConnectionProvider, SensorServiceConnectionProvider>();
            IocManager.Register<ISensorServiceManager, SensorServiceManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
