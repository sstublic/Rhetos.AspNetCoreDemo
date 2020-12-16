using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Rhetos.Extensibility;
using Rhetos.Extensions.NetCore.Logging;
using Rhetos.Logging;
using Rhetos.Utilities;

namespace Rhetos.Extensions.NetCore
{
    public class RhetosHostBuilder
    {
        private readonly Lazy<IConfigurationBuilder> configurationBuilder;
        private ILogProvider builderLogProvider = new RhetosBuilderDefaultLogProvider();
        private readonly List<Action<ContainerBuilder>> configureContainerActions = new List<Action<ContainerBuilder>>();
        private string rhetosAppPath;

        public RhetosHostBuilder()
        {
            configurationBuilder = new Lazy<IConfigurationBuilder>(() =>
            {
                var builder = new ConfigurationBuilder(builderLogProvider);
                builder.AddKeyValue("Rhetos:PluginScanner:IgnoreAssemblyFiles:0", "rhetos.exe");
                builder.AddKeyValue("Rhetos:PluginScanner:IgnoreAssemblyFiles:1", "rhetos.dll");
                builder.AddKeyValue("Rhetos:App:AssetsFolder", "RhetosAssets");
                return builder;
            });
        }

        public RhetosHostBuilder UseBuilderLogProvider(ILogProvider logProvider)
        {
#warning inconsistent initialization of logProviders: if configuration builder is evaluated before UseBuilderLogProvider is called, two different LogProviders will be used
            if (configurationBuilder.IsValueCreated)
                throw new InvalidOperationException($"Changing log providers is allowed only before modifying configuration.");

            builderLogProvider = logProvider;
            return this;
        }

        public RhetosHostBuilder UseRhetosApp(string appPath)
        {
            if (!appPath.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                appPath += ".dll";

            rhetosAppPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, appPath));
            return this;
        }

        public RhetosHostBuilder UseUserInfoProvider(Func<IUserInfo> userInfoProvider)
        {
            ConfigureContainer(container => container.Register(_ => userInfoProvider()).As<IUserInfo>().InstancePerLifetimeScope());
            return this;
        }

        public RhetosHostBuilder ConfigureConfiguration(Action<IConfigurationBuilder> configureAction)
        {
            configureAction?.Invoke(configurationBuilder.Value);
            return this;
        }

        public RhetosHostBuilder ConfigureContainer(Action<ContainerBuilder> configureAction)
        {
            configureContainerActions.Add(configureAction);
            return this;
        }

        public RhetosHost Build()
        {
            var rhetosRuntime = ResolveRhetosRuntime();

            var configuration = configurationBuilder.Value.Build();
            var rhetosContainer = rhetosRuntime.BuildContainer(builderLogProvider, configuration, ContainerConfigureAll);

            return new RhetosHost(rhetosContainer);
        }

        private void ContainerConfigureAll(ContainerBuilder rhetosContainerBuilder)
        {
            foreach (var configureContainerAction in configureContainerActions)
                configureContainerAction.Invoke(rhetosContainerBuilder);
        }

        private IRhetosRuntime ResolveRhetosRuntime()
        {
            var log = builderLogProvider.GetLogger("RhetosHost");
            var resolvedPath = rhetosAppPath;

            IRhetosRuntime rhetosRuntime;
            if (string.IsNullOrEmpty(resolvedPath))
            {
                log.Info(() => $"Rhetos app not specified, searching for {nameof(IRhetosRuntime)} implementations in base directory.");
                rhetosRuntime = TryFindRhetosRuntimeImplementation();
                if (rhetosRuntime == null)
                    throw new FrameworkException($"No implementation of interface {nameof(IRhetosRuntime)} found with Export attribute.");

                resolvedPath = rhetosRuntime.GetType().Assembly.Location;
                log.Info(() => $"Found {nameof(IRhetosRuntime)} in '{resolvedPath}'.");
            }
            else
            {
                log.Info(() => $"Rhetos app explicitly set, using default {nameof(IRhetosRuntime)} implementation to construct Rhetos container.");
                rhetosRuntime = new DefaultRhetosRuntime();
            }

            var runtimePathKey = $"{OptionsAttribute.GetConfigurationPath<RhetosAppOptions>()}:{nameof(RhetosAppOptions.RhetosRuntimePath)}";
            configurationBuilder.Value.AddKeyValue(runtimePathKey, resolvedPath);

            return rhetosRuntime;
        }

        private IRhetosRuntime TryFindRhetosRuntimeImplementation()
        {
            var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
            var assemblyPaths = Directory.GetFiles(baseFolder, "*.dll", SearchOption.TopDirectoryOnly)
                .ToArray();
            var assemblyResolver = AssemblyResolver.GetResolveEventHandler(assemblyPaths, builderLogProvider, true);
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver;

            try
            {
                var pluginScanner = new PluginScanner(assemblyPaths, baseFolder, builderLogProvider, new PluginScannerOptions());
                var rhetosRuntimeTypes = pluginScanner.FindPlugins(typeof(IRhetosRuntime)).Select(x => x.Type).ToList();

                if (rhetosRuntimeTypes.Count == 0)
                    return null;

                if (rhetosRuntimeTypes.Count > 1)
                    throw new FrameworkException($"Found multiple implementation of the type {nameof(IRhetosRuntime)}.");

                var rhetosRuntimeInstance = (IRhetosRuntime)Activator.CreateInstance(rhetosRuntimeTypes.Single());
                return rhetosRuntimeInstance;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolver;
            }
        }
    }
}
