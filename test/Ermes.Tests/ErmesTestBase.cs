using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.TestBase;
using Abp.UI;
using Azure.Core;
using Ermes.EntityFrameworkCore;
using Ermes.Roles.Dto;
using Ermes.Tests.TestDatas;
using FusionAuthNetCore;
using io.fusionauth.domain.api;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Ermes.Tests
{
    public class ErmesTestBase : AbpIntegratedTestBase<ErmesTestModule>
    {
        public const string USERNAME_OM = "organization.manager.1";
        public const string USERNAME_OM_CHILD = "organization.manager.child.1";
        public const string USERNAME_FR = "first.responder.1";
        public const string USERNAME_ADMIN = "admin";
        public const string USERNAME_CITIZEN = "citizen.1";
        public const string BASE_QUERY_PARAMS = "?maxResultCount=100";

        public ErmesTestBase()
        {
            UsingDbContext(context => new TestDataBuilder(context).Build());
        }

        public HttpClient GetApiClient(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", token);
            client.BaseAddress = new Uri("https://api-demo.shelter-project.cloud/api/services/app/");
            return client;
        }

        public async Task<string> SendHttpRequest(HttpRequestMessage request, HttpClient client)
        {
            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }

            var responseValue = string.Empty;
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                Task task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                {
                    var stream = t.Result;
                    using var reader = new StreamReader(stream);
                    responseValue = reader.ReadToEnd();
                });

                task.Wait();
                return responseValue;
            }
            else
                return null;
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

        public async static Task<string> GetToken(string username, string password = "shelter2020")
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

            if (response.WasSuccessful())
                return response.successResponse.token;
            else
                return null;
        }

        
    }
}
