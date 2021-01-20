using Abp.IO;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Ermes.Localization;
using System.IO;

namespace Ermes
{
    public class ErmesCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            ErmesLocalizationConfigurer.Configure(Configuration.Localization);
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