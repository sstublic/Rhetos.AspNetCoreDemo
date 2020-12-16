using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using Rhetos.Extensibility;
using Rhetos.Extensions.NetCore.Logging;
using Rhetos.Logging;
using Rhetos.Utilities;

namespace Rhetos.Extensions.NetCore
{
    public class RhetosHostBuilder
    {
        private ILogProvider builderLogProvider = new RhetosBuilderDefaultLogProvider();
        private readonly List<Action<ContainerBuilder>> configureContainerActions = new List<Action<ContainerBuilder>>();
        private readonly List<Action<IConfigurationBuilder>> configureConfigurationActions = new List<Action<IConfigurationBuilder>>();
        private string rhetosAppSettingsFilePath;
        private ILogger buildLogger;
        private bool allowDefaultRhetosRuntime = false;

        public RhetosHostBuilder()
        {
        }

        public RhetosHostBuilder UseBuilderLogProvider(ILogProvider logProvider)
        {
            builderLogProvider = logProvider;
            return this;
        }

        public RhetosHostBuilder WithDefaultRhetosRuntime(bool enabled)
        {
            allowDefaultRhetosRuntime = enabled;
            return this;
        }

        public RhetosHostBuilder UseRhetosAppSettingsFile(string appSettingsFilePath)
        {
            rhetosAppSettingsFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, appSettingsFilePath));
            return this;
        }

        public RhetosHostBuilder UseUserInfoProvider(Func<IUserInfo> userInfoProvider)
        {
            ConfigureContainer(container => container.Register(_ => userInfoProvider()).As<IUserInfo>().InstancePerLifetimeScope());
            return this;
        }

        public RhetosHostBuilder ConfigureConfiguration(Action<IConfigurationBuilder> configureAction)
        {
            configureConfigurationActions.Add(configureAction);
            return this;
        }

        public RhetosHostBuilder ConfigureContainer(Action<ContainerBuilder> configureAction)
        {
            configureContainerActions.Add(configureAction);
            return this;
        }

        public RhetosHost Build()
        {
            buildLogger = builderLogProvider.GetLogger(nameof(RhetosHost));
            try
            {
                var configuration = CreateConfiguration();

                var appOptions = configuration.GetOptions<RhetosAppOptions>();
                var rhetosRuntime = ResolveRhetosRuntimeInstance(appOptions.RhetosRuntimePath);
                if (rhetosRuntime == null)
                {
                    if (!allowDefaultRhetosRuntime)
                        throw new FrameworkException($"{nameof(IRhetosRuntime)} implementation/export not found in '{appOptions.RhetosRuntimePath}'"
                                                     + $" and {nameof(DefaultRhetosRuntime)} is not enabled."
                                                     + $" Add {nameof(IRhetosRuntime)} implementation or enable default by .{nameof(WithDefaultRhetosRuntime)}(true).");

                    buildLogger.Info(() => $"{nameof(IRhetosRuntime)} implementation not found in '{appOptions.RhetosRuntimePath}'. Using {nameof(DefaultRhetosRuntime)}.");
                    rhetosRuntime = new DefaultRhetosRuntime();
                }

                var rhetosContainer = rhetosRuntime.BuildContainer(builderLogProvider, configuration, ContainerConfigureAll);

                return new RhetosHost(rhetosContainer);
            }
            catch (Exception e)
            {
                buildLogger.Error(() => $"Error during {nameof(RhetosHostBuilder)}.{nameof(Build)}: {e}");
                throw;
            }
        }

        private IConfiguration CreateConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder(builderLogProvider);
            rhetosAppSettingsFilePath ??= Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RhetosAppEnvironment.ConfigurationFileName));
            if (!File.Exists(rhetosAppSettingsFilePath))
                throw new FrameworkException($"Unable to initialize RhetosHost. Rhetos app settings file '{rhetosAppSettingsFilePath}' not found.");

            buildLogger.Info(() => $"Initializing Rhetos app from '{rhetosAppSettingsFilePath}'.");
            configurationBuilder.AddJsonFile(rhetosAppSettingsFilePath);
            
            foreach (var configureConfigurationAction in configureConfigurationActions)
                configureConfigurationAction.Invoke(configurationBuilder);

            return configurationBuilder.Build();
        }

        private void ContainerConfigureAll(ContainerBuilder rhetosContainerBuilder)
        {
            foreach (var configureContainerAction in configureContainerActions)
                configureContainerAction.Invoke(rhetosContainerBuilder);
        }

        private IRhetosRuntime ResolveRhetosRuntimeInstance(string rhetosRuntimePath)
        {
            var assemblyResolver = AssemblyResolver.GetResolveEventHandler(new[] { rhetosRuntimePath }, builderLogProvider, true);
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver;

            try
            {
                var pluginScanner = new PluginScanner(new[] { rhetosRuntimePath }, Path.GetDirectoryName(rhetosRuntimePath), builderLogProvider, new PluginScannerOptions());
                var rhetosRuntimeTypes = pluginScanner.FindPlugins(typeof(IRhetosRuntime)).Select(x => x.Type).ToList();

                if (rhetosRuntimeTypes.Count == 0)
                    return null;

                if (rhetosRuntimeTypes.Count > 1)
                    throw new FrameworkException($"Found multiple implementation of the type {nameof(IRhetosRuntime)}.");

                return (IRhetosRuntime)Activator.CreateInstance(rhetosRuntimeTypes.Single());
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolver;
            }
        }
    }
}
