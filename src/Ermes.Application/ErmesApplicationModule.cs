using Abp.AutoMapper;
using Abp.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Session;
using Abp.Configuration.Startup;
using Ermes.Attributes;
using Abp.Authorization;
using Ermes.Authorization;
using Abp.SocialMedia;
using Abp.CsiServices;

namespace Ermes
{
    [DependsOn(
        typeof(ErmesCoreModule), 
        typeof(AbpAutoMapperModule),
        typeof(AbpSocialMediaModule),
        typeof(AbpCsiModule))]
    public class ErmesApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            ErmesInterceptorRegistrar.Initialize(IocManager);
            Configuration.ReplaceService<IAbpSession, ErmesAppSession>(Abp.Dependency.DependencyLifeStyle.Singleton);
            Configuration.ReplaceService<IPermissionChecker, ErmesPermissionChecker>(Abp.Dependency.DependencyLifeStyle.Singleton);
            //Configuration.ReplaceService<IAuthorizationHelper, ErmesAuthorizationHelper>(Abp.Dependency.DependencyLifeStyle.Singleton);
            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
            {
                CustomDtoMapper.CreateMappings(configuration, new MultiLingualMapContext(
                    IocManager.Resolve<ISettingManager>()
                ));
            });
        }
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ErmesApplicationModule).GetAssembly());
        }
    }
}