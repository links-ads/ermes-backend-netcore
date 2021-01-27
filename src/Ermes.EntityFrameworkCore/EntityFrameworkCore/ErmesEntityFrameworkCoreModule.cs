using Abp;
using Abp.Domain.Entities.Auditing;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Ermes.EntityFrameworkCore
{
    [DependsOn(
        typeof(ErmesCoreModule), 
        typeof(AbpEntityFrameworkCoreModule))]
    public class ErmesEntityFrameworkCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            // Set this setting to true for enabling entity history.
            Configuration.EntityHistory.IsEnabled = true;
            Configuration.EntityHistory.IsEnabledForAnonymousUsers = true;
            // Uncomment below line to write change logs for the entities below:
            Configuration.EntityHistory.Selectors.Add(
                new NamedTypeSelector(
                    "Abp.AuditedEntity",
                    type => typeof(AuditedEntity).IsAssignableFrom(type)
                )
            );
            // Configuration.CustomConfigProviders.Add(new EntityHistoryConfigProvider(Configuration));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ErmesEntityFrameworkCoreModule).GetAssembly());
        }
    }
}