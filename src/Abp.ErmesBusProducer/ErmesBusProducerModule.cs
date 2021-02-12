using Abp.Modules;
using Abp.ErmesBusProducer.Configuration;
using System.Reflection;
using System.Configuration;
using Abp.ErmesBusProducer.Kafka;
using Abp.ErmesBusProducer.RabbitMq;

namespace Abp.ErmesBusProducer
{
    [DependsOn(typeof(AbpKernelModule))]
    public class ErmesBusProducerModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<ErmesBusProducerSettings, ErmesBusProducerSettings>();
            IocManager.Register<IErmesBusConfigurationProvider, ErmesBusConfigurationProvider>();

            //setting to be added in app.config file in Ermes.Web project
            string project = ConfigurationManager.AppSettings["ERMES_PROJECT"];
            switch (project)
            {
                case "SHELTER":
                    IocManager.Register<IErmesBusProducer, ErmesRabbitMqManager>();
                    break;
                case "FASTER":
                case "SAFERS":
                    IocManager.Register<IErmesBusProducer, KafkaProducer>();
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
