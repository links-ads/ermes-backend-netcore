using Abp.Modules;
using Abp.BusProducer.Configuration;
using System.Reflection;
using System.Configuration;
using Abp.BusProducer.Kafka;
using Abp.BusProducer.RabbitMq;

namespace Abp.BusProducer
{
    [DependsOn(typeof(AbpKernelModule))]
    public class BusProducerModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<BusProducerSettings, BusProducerSettings>();
            IocManager.Register<IBusConfigurationProvider, BusConfigurationProvider>();

            //setting to be added in app.config file in Ermes.Web project
            string project = ConfigurationManager.AppSettings["ERMES_PROJECT"];
            switch (project)
            {
                case "SHELTER":
                case "SAFERS":
                    IocManager.Register<IBusProducer, RabbitMqManager>();
                    break;
                case "FASTER":
                    IocManager.Register<IBusProducer, KafkaProducer>();
                    break;
                default:
                    break;
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
