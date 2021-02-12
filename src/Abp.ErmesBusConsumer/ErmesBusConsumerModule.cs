using Abp.Modules;
using System.Reflection;
using System.Configuration;
using Abp.ErmesBusConsumer.Kafka;
using Abp.ErmesBusConsumer.RabbitMq;
using Abp.ErmesBusConsumer.Configuration;

namespace Abp.ErmesBusConsumer
{
    [DependsOn(typeof(AbpKernelModule))]
    public class ErmesBusConsumerModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<ErmesBusConsumerSettings, ErmesBusConsumerSettings>();
            IocManager.Register<IErmesBusConfigurationProvider, ErmesBusConfigurationProvider>();

            //setting to be added in app.config file in Ermes.Web project
            //string project = ConfigurationManager.AppSettings["ERMES_PROJECT"];
            //switch (project)
            //{
            //    case "SHELTER":
            //        IocManager.Register<IErmesBusConsumer, ErmesRabbitMqManager>();
            //        break;
            //    case "FASTER":
            //    case "SAFERS":
            //        IocManager.Register<IErmesBusConsumer, KafkaConsumer>();
            //        break;
            //    default:
            //        break;
            //}
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
