﻿using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Ermes.Configuration;
using Ermes.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;

namespace Ermes.Web.Startup
{
    [DependsOn(
        typeof(ErmesApplicationModule), 
        typeof(ErmesEntityFrameworkCoreModule), 
        typeof(AbpAspNetCoreModule))]
    public class ErmesWebModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public ErmesWebModule(IWebHostEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(ErmesConsts.ConnectionStringName);

            Configuration.Navigation.Providers.Add<ErmesNavigationProvider>();

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(ErmesApplicationModule).GetAssembly()
                );
            Configuration.Modules.AbpAspNetCore().DefaultWrapResultAttribute.WrapOnSuccess = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ErmesWebModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(ErmesWebModule).Assembly);
        }
    }
}