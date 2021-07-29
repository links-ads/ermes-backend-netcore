using Abp;
using Abp.CsiServices.Csi;
using Abp.CsiServices.Csi.Configuration;
using Abp.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Abp.CsiServices
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpCsiModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpCsiSettings, AbpCsiSettings>();
            IocManager.Register<ICsiConnectionProvider, CsiConnectionProvider>();
            IocManager.Register<ICsiManager, CsiManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
