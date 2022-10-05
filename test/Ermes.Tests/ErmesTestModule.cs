using System.Reflection;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Ermes.EntityFrameworkCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ermes.Tests
{
    [DependsOn(
        typeof(ErmesApplicationModule),
        typeof(ErmesEntityFrameworkCoreModule),
        typeof(AbpTestBaseModule)
        )]
    public class ErmesTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
            SetupInMemoryDb();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ErmesTestModule).GetAssembly());
        }

        private void SetupInMemoryDb()
        {
            var services = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
                IocManager.IocContainer,
                services
            );

            var builder = new DbContextOptionsBuilder<ErmesDbContext>();

            //builder.UseInMemoryDatabase("Test").UseInternalServiceProvider(serviceProvider);
            //There's no possibility at the momento of writing to have a in-memory version of the PostgreSQL database
            string connectionString = "Server=localhost; Database=safers; User ID=safersadmin; Password=linksfoundation_2021!; Port=5452;";
            builder.UseNpgsql(connectionString,
                x => x.UseNetTopologySuite(geographyAsDefault: true)
            );

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<ErmesDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton()
            );
        }
    }
}