using System;
using System.Linq;
using System.Text.RegularExpressions;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rhetos;
using Rhetos.Extensions.AspNetCore;
using Rhetos.Processing;
using Rhetos.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RhetosAspNetCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddRhetos(this IServiceCollection serviceCollection, string rhetosApplicationFolder,
            Microsoft.Extensions.Configuration.IConfiguration configurationToMap = null)
        {
            var logProvider = new ConsoleLogProvider();
            var host = Host.Find(rhetosApplicationFolder, logProvider);

            var rhetosConfiguration = host.RhetosRuntime.BuildConfiguration(logProvider, rhetosApplicationFolder, builder =>
            {
                if (configurationToMap != null)
                {
                    foreach (var configurationItem in configurationToMap.AsEnumerable().Where(a => a.Value != null))
                    {
                        var key = configurationItem.Key;
                        if (configurationToMap is IConfigurationSection configurationSection && !string.IsNullOrEmpty(configurationSection.Path))
                        {
                            var regex = new Regex($"^{configurationSection.Path}:");
                            key = regex.Replace(key, "");
                        }

                        // Console.WriteLine($"Mapping '{key}':'{configurationItem.Value}'");
                        builder.AddKeyValue(key, configurationItem.Value.Replace("ILLIDAN", Environment.MachineName));
                    }
                }
            });
            serviceCollection.AddHttpContextAccessor();

            /*
            foreach (var key in rhetosConfiguration.AllKeys)
            {
                Console.WriteLine($"{key}: '{rhetosConfiguration.GetValue<string>(key)}'");
            }
            Console.WriteLine($"Machine: '{Environment.MachineName}'");
            */

            var rhetosContainer = host.RhetosRuntime.BuildContainer(logProvider, rhetosConfiguration, _ => { });
            serviceCollection.AddSingleton(new RhetosRootServiceProvider(rhetosContainer));
            serviceCollection.AddScoped<RhetosScopeServiceProvider>();
            serviceCollection.AddRhetosComponent<Lazy<IProcessingEngine>>();
            
            return serviceCollection;
        }

        public static IServiceCollection AddRhetosComponent<T>(this IServiceCollection serviceCollection) where T : class
        {
            serviceCollection.AddScoped(services => services
                .GetRequiredService<RhetosScopeServiceProvider>()
                .Resolve<T>());
            return serviceCollection;
        }
    }
}
