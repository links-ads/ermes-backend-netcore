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
