using System;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.EntityFrameworkCore;
using Ermes.EntityFrameworkCore;
using Castle.Facilities.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Abp.Timing;
using Ermes.Configuration;
using Ermes.Web.Converters;
using System.Linq;
using Ermes.Web.Middlewares;
using NSwag;
using NSwag.Generation.Processors.Security;
using NSwag.AspNetCore;
using Abp.Extensions;
using io.fusionauth;
using FusionAuthNetCore;

namespace Ermes.Web.Startup
{
    public class Startup
    {
        private readonly IConfigurationRoot _appConfiguration;
        readonly string DefaultCorsPolicyName = "localhost";
        private FusionAuthClient client;

        public Startup(IWebHostEnvironment env)
        {
            Clock.Provider = ClockProviders.Utc;
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            client = new FusionAuthClient(_appConfiguration["FusionAuth:ApiKey"], _appConfiguration["FusionAuth:Url"], _appConfiguration["FusionAuth:TenantId"]);

            //Configure DbContext
            services.AddAbpDbContext<ErmesDbContext>(options =>
            {
                DbContextOptionsConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
            });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new GeometryJsonConverter());
            });

            AuthConfigurer.Configure(services, _appConfiguration);

            services.Configure<FusionAuthSettings>(
                _appConfiguration.GetSection("FusionAuth")
            );

            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    //App:CorsOrigins in appsettings.json can contain more than one address with splitted by comma.
                    builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddOpenApiDocument(options => {
                options.DocumentName = ErmesConsts.SwaggerAppDocumentName;
                options.Version = "v1";
                options.Title = "Project APIs";
                options.Description = "List of all APIs available in the backend";
                options.ApiGroupNames = new string[] { ErmesConsts.SwaggerAppDocumentName };
                options.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
                {
                    Description = "Nella casella sottostante digita il token ottenuto da /api/Token. Esempio: <strong><code>eyJhbGci...</code></strong><br><br><br><br>",
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Type = OpenApiSecuritySchemeType.ApiKey
                });

                options.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            services.AddOpenApiDocument(options => {
                options.DocumentName = ErmesConsts.SwaggerBackofficeDocumentName;
                options.Version = "v1";
                options.Title = "Backoffice APIs";
                options.Description = "List of all APIs for backoffice operations";
                options.ApiGroupNames = new string[] { ErmesConsts.SwaggerBackofficeDocumentName };
                options.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
                {
                    Description = "Nella casella sottostante digita il token ottenuto da /api/Token. Esempio: <strong><code>eyJhbGci...</code></strong><br><br><br><br>",
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Type = OpenApiSecuritySchemeType.ApiKey
                });

                options.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });

            //Configure Abp and Dependency Injection
            return services.AddAbp<ErmesWebModule>(options =>
            {
                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                );
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(); //Initializes ABP framework.

            if (
                env.IsEnvironment("ShelterLocal") ||
                env.IsEnvironment("ShelterDevelopment") ||
                env.IsEnvironment("FasterLocal") ||
                env.IsEnvironment("FasterDevelopment")
            )
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseMiddleware<JwtTokenMiddleware>(client);
            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors(DefaultCorsPolicyName);
            app.UseEndpoints(endpoints => {
                endpoints.MapDefaultControllerRoute();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.SwaggerRoutes.Add(new SwaggerUi3Route(ErmesConsts.SwaggerAppDocumentName, "/swagger/app-v1/swagger.json"));
                options.SwaggerRoutes.Add(new SwaggerUi3Route(ErmesConsts.SwaggerBackofficeDocumentName, "/swagger/backoffice-v1/swagger.json"));
            });
        }
    }
}
