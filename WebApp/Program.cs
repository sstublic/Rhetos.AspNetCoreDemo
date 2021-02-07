using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Rhetos;
using Rhetos.Dom;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensions.RestApi;
using ConfigurationBuilder = Rhetos.ConfigurationBuilder;

namespace WebApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            /*
            Sandbox();
            return;
            */
            var host = CreateHostBuilder(args)
                .Build();
            
            host.Run();
        }

        public static void Sandbox()
        {
            var host = CreateHostBuilder(null).Build();
            var rhetosHost = host.Services.GetRequiredService<RhetosHost>();

            var dslModel = DslModelRestAspect.ResolveFromHost<IDslModel>(rhetosHost);
            var objectModel = DslModelRestAspect.ResolveFromHost<IDomainObjectModel>(rhetosHost);

            Console.WriteLine(objectModel.Assemblies.Count());
            var assembly = objectModel.Assemblies.Single();
            Console.WriteLine(assembly.FullName);

            foreach (var conceptInfo in dslModel.Concepts)
            {
                var type = assembly.GetType(conceptInfo.GetKeyProperties());
                if (conceptInfo is DataStructureInfo)
                    Console.WriteLine($"{conceptInfo.GetKeyProperties(),-40}: {type?.FullName ?? "NULL",-40} [{type?.Assembly.FullName}]");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseUrls("http://0.0.0.0:80");
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
