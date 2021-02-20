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
            //Sandbox();
            //return;
            
            var host = CreateHostBuilder(args)
                .Build();
            
            host.Run();
        }

        // TODO remove debug code
        public static void Sandbox()
        {
            var host = CreateHostBuilder(null).Build();
            var rhetosHost = host.Services.GetRequiredService<RhetosHost>();

            var dslModel = rhetosHost.GetRootContainer().Resolve<IDslModel>();
            var objectModel = rhetosHost.GetRootContainer().Resolve<IDomainObjectModel>();

            foreach (var conceptInfo in dslModel.Concepts.Where(a => a.GetKeyProperties().Contains("HighValueParam")))
            {
                var conceptType = objectModel.Assemblies.Single().GetType(conceptInfo.ToString());
                //Console.WriteLine($"{GetBaseTypes(conceptInfo.GetType())}, {conceptInfo.GetKeyProperties()}, {conceptInfo is IWritableOrmDataStructure}, {conceptType?.FullName}, {conceptType is IWritableOrmDataStructure}");
                Console.WriteLine($"{conceptInfo.GetKeyProperties()}, {conceptInfo.GetKey()}, {conceptInfo.GetKeywordOrTypeName()}");
            }

            /*
            Console.WriteLine(objectModel.Assemblies.Count());
            var assembly = objectModel.Assemblies.Single();
            Console.WriteLine(assembly.FullName);

            foreach (var conceptInfo in dslModel.Concepts)
            {
                var type = assembly.GetType(conceptInfo.GetKeyProperties());
                if (conceptInfo is DataStructureInfo)
                    Console.WriteLine($"{conceptInfo.GetKeyProperties(),-40}: {type?.FullName ?? "NULL",-40} [{type?.Assembly.FullName}]");
            }*/
        }

        // TODO remove debug code
        private static string GetBaseTypes(Type type)
        {
            var baseTypes = new List<string>();
            while (type != null)
            {
                baseTypes.Add(type.Name);
                type = type.BaseType;
            }

            return string.Join("->", baseTypes);
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
