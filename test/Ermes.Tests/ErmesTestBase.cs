using System;
using System.Threading.Tasks;
using Abp.TestBase;
using Ermes.EntityFrameworkCore;
using Ermes.Tests.TestDatas;
using FusionAuthNetCore;
using io.fusionauth.domain.api;
using Microsoft.Extensions.Configuration;

namespace Ermes.Tests
{
    public class ErmesTestBase : AbpIntegratedTestBase<ErmesTestModule>
    {
        public ErmesTestBase()
        {
            UsingDbContext(context => new TestDataBuilder(context).Build());
        }

        protected virtual void UsingDbContext(Action<ErmesDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<ErmesDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected virtual T UsingDbContext<T>(Func<ErmesDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<ErmesDbContext>())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected virtual async Task UsingDbContextAsync(Func<ErmesDbContext, Task> action)
        {
            using (var context = LocalIocManager.Resolve<ErmesDbContext>())
            {
                await action(context);
                await context.SaveChangesAsync(true);
            }
        }

        protected virtual async Task<T> UsingDbContextAsync<T>(Func<ErmesDbContext, Task<T>> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<ErmesDbContext>())
            {
                result = await func(context);
                context.SaveChanges();
            }

            return result;
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }

        public async static Task<string> GetToken(string username, string password = "safers2020")
        {
            var config = InitConfiguration();
            var settings = new FusionAuthSettings()
            {
                ApplicationId = config["FusionAuth:ApplicationId"],
                Tenant = config["FusionAuth:Tenant"],
                TenantId = config["FusionAuth:TenantId"],
                Url = config["FusionAuth:Url"],
                ApiKey = config["FusionAuth:ApiKey"],
            };
            var client = FusionAuth.GetFusionAuthClient(settings);
            var response = await client.LoginAsync(new LoginRequest()
                            .with(lr => lr.applicationId = new Guid(settings.ApplicationId))
                            .with(lr => lr.loginId = username)
                            .with(lr => lr.password = password));

            return response.successResponse.token;
        }
    }
}
