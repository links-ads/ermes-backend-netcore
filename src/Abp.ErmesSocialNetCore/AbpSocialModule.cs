using Abp;
using Abp.ErmesSocialNetCore.Social;
using Abp.ErmesSocialNetCore.Social.Configuration;
using Abp.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Abp.ErmesSocialNetCore
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpSocialModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpSocialSettings, AbpSocialSettings>();
            IocManager.Register<ISocialConnectionProvider, SocialConnectionProvider>();
            IocManager.Register<ISocialManager, SocialManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
