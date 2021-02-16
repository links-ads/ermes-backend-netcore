using Abp.Modules;
using Abp.SocialMedia.Configuration;
using System;
using System.Reflection;

namespace Abp.SocialMedia
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpSocialMediaModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<AbpSocialMediaSettings, AbpSocialMediaSettings>();
            IocManager.Register<ISocialMediaConnectionProvider, SocialMediaConnectionProvider>();
            IocManager.Register<ISocialMediaManager, SocialMediaManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
