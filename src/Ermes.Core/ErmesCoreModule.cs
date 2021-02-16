using Abp.Azure;
using Abp.Chatbot;
using Abp.BusProducer;
using Abp.Firebase;
using Abp.IO;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Ermes.Configuration;
using Ermes.Localization;
using System.IO;

namespace Ermes
{
    [DependsOn(
        typeof(AbpAzureModule),
        typeof(AbpFirebaseModule),
        typeof(AbpChatbotModule),
        typeof(BusProducerModule)
    )]
    public class ErmesCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            ErmesLocalizationConfigurer.Configure(Configuration.Localization);

            //Adding setting providers
            Configuration.Settings.Providers.Add<AppSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ErmesCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            SetAppFolders();
        }

        private void SetAppFolders()
        {
            var appFolders = IocManager.Resolve<AppFolders>();

            appFolders.TempFileDownloadFolder = Path.Combine("wwwroot", $"Temp{Path.DirectorySeparatorChar}Downloads");
            appFolders.WebLogsFolder = Path.Combine("App_Data", "Logs");

            try
            {
                DirectoryHelper.CreateIfNotExists(appFolders.TempFileDownloadFolder);
                DirectoryHelper.CreateIfNotExists(appFolders.WebLogsFolder);
            }
            catch { }
        }
    }
}