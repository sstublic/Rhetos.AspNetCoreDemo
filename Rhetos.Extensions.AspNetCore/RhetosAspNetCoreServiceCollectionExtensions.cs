using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Rhetos;
using Rhetos.Extensions.AspNetCore;
using Rhetos.Utilities;
using IConfigurationBuilder = Rhetos.IConfigurationBuilder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RhetosAspNetCoreServiceCollectionExtensions
    {
        public static RhetosServiceCollectionBuilder AddRhetos(this IServiceCollection serviceCollection, string rhetosApplicationFolder,
            Action<RhetosServiceCollectionBuilder> builderActions = null)
        {
            return AddRhetos(serviceCollection, rhetosApplicationFolder, null, builderActions);
        }

        public static RhetosServiceCollectionBuilder AddRhetos(this IServiceCollection serviceCollection, string rhetosApplicationFolder,
            Configuration.IConfiguration configurationToMap = null, Action<RhetosServiceCollectionBuilder> builderActions = null)
        {
            var logProvider = new ConsoleLogProvider();
            var rhetosHost = Host.Find(rhetosApplicationFolder, logProvider);

            var rhetosConfiguration = rhetosHost.RhetosRuntime.BuildConfiguration(logProvider, rhetosApplicationFolder,
                builder => MapAspNetCoreConfiguration(builder, configurationToMap));
            var rhetosContainer = rhetosHost.RhetosRuntime.BuildContainer(logProvider, rhetosConfiguration, _ => { });

            serviceCollection.AddHttpContextAccessor();
            serviceCollection.AddSingleton(new RhetosRootServiceProvider(rhetosContainer));
            serviceCollection.AddScoped<RhetosScopeServiceProvider>();

            var rhetosServiceCollectionBuilder = new RhetosServiceCollectionBuilder(serviceCollection);
            builderActions?.Invoke(rhetosServiceCollectionBuilder);

            return rhetosServiceCollectionBuilder;
        }

        private static void MapAspNetCoreConfiguration(IConfigurationBuilder builder, Configuration.IConfiguration configurationToMap)
        {
            if (configurationToMap == null) return;
            foreach (var configurationItem in configurationToMap.AsEnumerable().Where(a => a.Value != null))
            {
                var key = configurationItem.Key;
                if (configurationToMap is IConfigurationSection configurationSection && !string.IsNullOrEmpty(configurationSection.Path))
                {
                    var regex = new Regex($"^{configurationSection.Path}:");
                    key = regex.Replace(key, "");
                }
                builder.AddKeyValue(key, configurationItem.Value);
            }
        }
    }
}
