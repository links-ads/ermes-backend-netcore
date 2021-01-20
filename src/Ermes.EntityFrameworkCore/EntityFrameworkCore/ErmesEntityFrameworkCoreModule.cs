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
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ErmesEntityFrameworkCoreModule).GetAssembly());
        }
    }
}