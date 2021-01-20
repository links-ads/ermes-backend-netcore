using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Ermes
{
    [DependsOn(
        typeof(ErmesCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ErmesApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ErmesApplicationModule).GetAssembly());
        }
    }
}