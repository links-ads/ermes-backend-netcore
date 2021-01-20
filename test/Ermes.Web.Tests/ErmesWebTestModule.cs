using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Ermes.Web.Startup;
namespace Ermes.Web.Tests
{
    [DependsOn(
        typeof(ErmesWebModule),
        typeof(AbpAspNetCoreTestBaseModule)
        )]
    public class ErmesWebTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ErmesWebTestModule).GetAssembly());
        }
    }
}