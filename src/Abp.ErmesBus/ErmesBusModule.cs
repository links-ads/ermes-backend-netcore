using Abp.Modules;
using Abp.Bus.Kafka;
using Abp.Bus.Configuration;
using System;
using System.Reflection;
using System.Configuration;

namespace Abp.Bus
{
    public class ErmesBusModule : AbpModule
    {
        public override void PreInitialize()
        {
            //setting to be added in app.config file in Ermes.Web project
            string project = ConfigurationManager.AppSettings["ERMES_PROJECT"];
            switch (project)
            {
                case "SHELTER":
                    IocManager.Register<IErmesBusManager, ErmesRabbitMqManager>();
                    break;
                case "FASTER":
                case "SAFERS":
                    IocManager.Register<IErmesBusManager, ErmesKafkaManager>();
                    break;
                default:
                    break;
            }

            IocManager.Register<ErmesBusSettings, ErmesBusSettings>();
            IocManager.Register<IErmesBusConfigurationProvider, ErmesBusConfigurationProvider>();

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
