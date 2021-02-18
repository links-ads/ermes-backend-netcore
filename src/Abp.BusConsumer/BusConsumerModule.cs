using Abp.Modules;
using System.Reflection;
using System.Configuration;
using Abp.BusConsumer.Kafka;
using Abp.BusConsumer.RabbitMq;
using Abp.BusConsumer.Configuration;
using Ermes;

namespace Abp.BusConsumer
{
    [DependsOn(
        typeof(AbpKernelModule),
        typeof(ErmesCoreModule)
    )]
    public class BusConsumerModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<BusConsumerSettings, BusConsumerSettings>();
            IocManager.Register<IBusConfigurationProvider, BusConfigurationProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
