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
            //
            /*
            var host = CreateHostBuilder(args).Build();
            var rhetosHost = host.Services.GetRequiredService<RhetosHost>();
            using (var rhetosScope = rhetosHost.CreateScope())
            {
                var dslModel = rhetosScope.Resolve<IDslModel>();
                var queryInfos = dslModel.Concepts.Where(a => a.GetKeyProperties().Contains("DemoEntity"))
                    .Select(a => (key: a.GetKeyProperties(), type: a.GetType(), isDataStructure: a is DataStructureInfo, supported: DslModelRestAspect.IsTypeSupported(a as DataStructureInfo)))
                    .ToList();
                var dataStructures = dslModel.Concepts.OfType<DataStructureInfo>().ToList();
                var domainObjectModel = rhetosScope.Resolve<IDomainObjectModel>();
            }
            */
            //
            CreateHostBuilder(args)
                .Build()
                .Run();
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
