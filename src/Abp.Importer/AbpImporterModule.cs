using Abp;
using Abp.Importer.Configuration;
using Abp.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Abp.Importer
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpImporterModule: AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpImporterSettings, AbpImporterSettings>();
            IocManager.Register<IImporterConnectionProvider, ImporterConnectionProvider>();
            IocManager.Register<IImporterManager, ImporterManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
