using Abp.Chatbot.Configuration;
using Abp.Modules;
using System.Reflection;

namespace Abp.Chatbot
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpChatbotModule: AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpChatbotSettings, AbpChatbotSettings>();
            IocManager.Register<IChatbotConnectionProvider, ChatbotConnectionProvider>();
            IocManager.Register<IChatbotManager, ChatbotManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
