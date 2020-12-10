using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Rhetos.Utilities;

namespace Rhetos.Extensions.AspNetCore
{
    public class RhetosServiceCollectionBuilder
    {
        private readonly IServiceCollection serviceCollection;
        public RhetosServiceCollectionBuilder(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public RhetosServiceCollectionBuilder AddRhetosComponent<T>() where T : class
        {
            if (typeof(T) == typeof(IUserInfo))
                throw new InvalidOperationException($"Adding explicit IUserInfo registration would result in circular dependency resolution."
                                                    + " Registering IUserInfo implementation is available via AddRhetosUserInfo<T>()");

            serviceCollection.AddScoped(services => services
                .GetRequiredService<RhetosScopeServiceProvider>()
                .Resolve<T>());
            return this;
        }

        public RhetosServiceCollectionBuilder AddAspNetCoreUser()
        {
            return AddRhetosUserInfo<RhetosAspNetCoreUser>();
        }

        public RhetosServiceCollectionBuilder AddRhetosUserInfo<T>() where T : class, IUserInfo
        {
            serviceCollection.AddScoped<IUserInfo, T>();
            return this;
        }
    }
}
