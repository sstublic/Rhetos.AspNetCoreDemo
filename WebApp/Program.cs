using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Rhetos;
using Rhetos.Dom;
using Rhetos.Dsl;

namespace WebApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args)
                .Build();
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        // This method exposes IRhetosHostBuilder for purposes of tooling using same configuration values
        // and same RhetosHostBuilder configuration delegate as the app itself
        public static IRhetosHostBuilder CreateRhetosHostBuilder()
        {
            // create Host for this web app
            var host = CreateHostBuilder(null).Build();
            
            // extract configuration of the web app
            var configuration = host.Services.GetRequiredService<IConfiguration>();

            // Create RhetosHostBuilder and configure it
            var rhetosHostBuilder = new RhetosHostBuilder();
            Startup.ConfigureRhetosHostBuilder(rhetosHostBuilder, configuration);

            return rhetosHostBuilder;
        }
    }
}
