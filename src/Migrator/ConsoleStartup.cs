using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.EntityFrameworkCore;
using Castle.Facilities.Logging;
using Ermes;
using Ermes.Configuration;
using Ermes.EntityFrameworkCore;
using Ermes.Web.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Migrator
{
    public class ConsoleStartup
    {
        private readonly IConfigurationRoot _appConfiguration;

        public ConsoleStartup(IWebHostEnvironment env)
        {
            _appConfiguration = AppConfigurations.Get(env.ContentRootPath, env.EnvironmentName);

            string filename;
            if (_appConfiguration["App:ErmesProject"] == AppConsts.Ermes_Safers)
            {
                switch (_appConfiguration["App:Environment"])
                {
                    case "dev":
                        filename = "appsettings.SafersDevelopment.json";
                        break;
                    case "test":
                        filename = "appsettings.SafersTest.json";
                        break;
                    case "local":
                    default:
                        filename = "appsettings.json";
                        break;
                }
            }
            else if(_appConfiguration["App:ErmesProject"] == AppConsts.Ermes_Shelter)
            {
                switch (_appConfiguration["App:Environment"])
                {
                    case "dev":
                        filename = "appsettings.ShelterDevelopment.json";
                        break;
                    case "test":
                        filename = "appsettings.ShelterTest.json";
                        break;
                    case "local":
                    default:
                        filename = "appsettings.json";
                        break;
                }
            }
            else
            {
                switch (_appConfiguration["App:Environment"])
                {
                    case "dev":
                        filename = "appsettings.FasterDevelopment.json";
                        break;
                    case "test":
                        filename = "appsettings.FasterTest.json";
                        break;
                    case "prod":
                        filename = "appsettings.FasterProduction.json";
                        break;
                    case "local":
                    default:
                        filename = "appsettings.json";
                        break;
                }
            }

            Console.WriteLine("Executing builder");
            var builder = new ConfigurationBuilder()
                .AddJsonFile(filename, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ErmesDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("Default"),
                    x => x.UseNetTopologySuite(geographyAsDefault: true)
                );
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}