using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Rhetos.Extensibility;
using Rhetos.Logging;
using Rhetos.Utilities;

namespace Rhetos.Extensions.NetCore
{
    public class DefaultRhetosRuntime : IRhetosRuntime
    {
        public IConfiguration BuildConfiguration(ILogProvider logProvider, string configurationFolder, Action<IConfigurationBuilder> addCustomConfiguration)
        {
            throw new NotImplementedException();
        }

        public IContainer BuildContainer(ILogProvider logProvider, IConfiguration configuration, Action<ContainerBuilder> registerCustomComponents)
        {
            var pluginAssemblies = AssemblyResolver.GetRuntimeAssemblies(configuration);
            var builder = new RhetosContainerBuilder(configuration, logProvider, pluginAssemblies);

            builder.AddRhetosRuntime();
            builder.AddPluginModules();

            registerCustomComponents?.Invoke(builder);

            return builder.Build();
        }
    }
}
