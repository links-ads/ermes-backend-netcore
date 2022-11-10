using Ermes.EntityFrameworkCore;
using Ermes.Web.Startup;
using io.fusionauth.domain;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Migrator
{
    internal class Program
    {
        /*
         * N.B.: the procedure does not work if ASPNETCORE_ENVIRONMENT is equals to Development.
         * In that case, this project throws the following exception:
         *      Cannot resolve scoped service from root provider
         * The explanation can be found here:
         *      https://github.com/benmccallum/fairybread/issues/43#issuecomment-826811950
         * For this reason, the value of the variable has been set to dev       
        */

        public static void Main(string[] args)
        {
            var wh = CreateWebHostBuilder(args).Build();
            using (var context = (ErmesDbContext)wh.Services.GetService(typeof(ErmesDbContext)))
            {
                if (context != null)
                    context.Database.Migrate();
            }
            Console.WriteLine("Done");
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                    .UseStartup<ConsoleStartup>();
    }
}
