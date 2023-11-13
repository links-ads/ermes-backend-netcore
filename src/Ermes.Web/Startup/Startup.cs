using Abp;
using Abp.AspNetCore;
using Abp.AzureCognitiveServices;
using Abp.BusConsumer;
using Abp.BusConsumer.Kafka;
using Abp.BusConsumer.RabbitMq;
using Abp.BusProducer;
using Abp.Castle.Logging.Log4Net;
using Abp.Chatbot;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using Abp.Importer;
using Abp.SensorService;
using Abp.SocialMedia;
using Abp.Timing;
using Castle.Facilities.Logging;
using Ermes.Configuration;
using Ermes.Dss;
using Ermes.EntityFrameworkCore;
using Ermes.ExternalServices.Csi.Configuration;
using Ermes.ExternalServices.Externals;
using Ermes.Web.App_Start;
using Ermes.Web.Converters;
using Ermes.Web.Middlewares;
using FusionAuthNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using io.fusionauth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using System;
using System.Linq;

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
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new GeometryJsonConverter());
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            AuthConfigurer.Configure(services, _appConfiguration);

            services.Configure<FusionAuthSettings>(
                _appConfiguration.GetSection("FusionAuth")
            );
            services.Configure<AbpAzureSettings>(
                _appConfiguration.GetSection("Azure")
            );
            services.Configure<AbpChatbotSettings>(
                _appConfiguration.GetSection("Chatbot")
            );
            services.Configure<AbpSocialMediaSettings>(
                _appConfiguration.GetSection("Social")
            );
            services.Configure<BusProducerSettings>(
                _appConfiguration.GetSection("Bus")
            );
            services.Configure<BusConsumerSettings>(
                _appConfiguration.GetSection("Bus")
            );
            services.Configure<AbpAzureCognitiveServicesSettings>(
                _appConfiguration.GetSection("AzureCognitiveServices")
            );
            services.Configure<ErmesSettings>(
                _appConfiguration.GetSection("App")
            );
            services.Configure<CsiSettings>(
                _appConfiguration.GetSection("Csi")
            );
            services.Configure<AbpImporterSettings>(
                _appConfiguration.GetSection("Importer")
            );
            services.Configure<AbpSensorServiceSettings>(
                _appConfiguration.GetSection("SensorService")
            );
            services.Configure<ExternalsSettings>(
                _appConfiguration.GetSection("Externals")
            );

            services.Configure<DssSettings>(
                _appConfiguration.GetSection("Dss")
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

            services.AddOpenApiDocument(options =>
            {
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

            services.AddOpenApiDocument(options =>
            {
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

                options.AddSecurity("ApiKey", Enumerable.Empty<string>(), new OpenApiSecurityScheme()
                {
                    Description = "Nella casella sottostante inserisci l'ApiKey. Esempio: <strong><code>eyJhbGci...</code></strong><br><br><br><br>",
                    Name = "X-API-Key",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Type = OpenApiSecuritySchemeType.ApiKey
                });

                options.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));

                options.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("ApiKey"));
            });

            if (bool.Parse(_appConfiguration["Bus:IsEnabled"]))
            {
                switch (_appConfiguration["Bus:Type"])
                {
                    case ErmesConsts.BusType.KAFKA:
                        services.AddHostedService<KafkaConsumer>();
                        break;
                    case ErmesConsts.BusType.RABBITMQ:
                        services.AddHostedService<RabbitMqConsumer>();
                        break;
                    default:
                        break;
                }
            }

            services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(_appConfiguration.GetConnectionString("Default"));
            });

            services.AddApplicationInsightsTelemetry();

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

            if (env.IsEnvironment("dev") || env.IsEnvironment("local"))
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseMiddleware<JwtTokenMiddleware>(client);
            app.UseMiddleware<ApiKeyMiddleware>();

            app.UseAuthentication();

            app.UseHangfireServer();


            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors(DefaultCorsPolicyName);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            if (bool.Parse(_appConfiguration["App:HangfireEnabled"]))
            {
                //Enable it to use HangFire dashboard (uncomment only if it's enabled in ErmesWebModule)
                app.UseHangfireDashboard("/hangfire", new DashboardOptions
                {
                    Authorization = new[] { new App_Start.AbpHangfireAuthorizationFilter() },
                    AppPath = "/swagger"
                });

                // Disable automatic retry for all hangfire job and disable concurrent execution
                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
                GlobalJobFilters.Filters.Add(new DisableConcurrentExecutionAttribute(60 * 5));

                // Setup recurring jobs
                JobsBootstrapper.SetupJobs();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3(options =>
            {
                options.SwaggerRoutes.Add(new SwaggerUi3Route(ErmesConsts.SwaggerAppDocumentName, "/swagger/app-v1/swagger.json"));
                options.SwaggerRoutes.Add(new SwaggerUi3Route(ErmesConsts.SwaggerBackofficeDocumentName, "/swagger/backoffice-v1/swagger.json"));
            });
        }
    }
}
