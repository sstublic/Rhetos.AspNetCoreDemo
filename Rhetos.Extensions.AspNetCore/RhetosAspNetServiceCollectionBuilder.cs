using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Rhetos.Utilities;

namespace Rhetos.Extensions.AspNetCore
{
    public class RhetosAspNetServiceCollectionBuilder
    {
        private readonly IServiceCollection serviceCollection;
        public RhetosAspNetServiceCollectionBuilder(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public RhetosAspNetServiceCollectionBuilder ExposeRhetosComponent<T>() where T : class
        {
            if (typeof(T) == typeof(IUserInfo))
                throw new InvalidOperationException($"Adding explicit IUserInfo registration would result in circular dependency resolution."
                                                    + " Register IUserInfo implementation via AddRhetosUserInfo<T>().");

            serviceCollection.AddScoped(serviceProvider => serviceProvider
                .GetRequiredService<RhetosScopeServiceProvider>()
                .Resolve<T>());

            return this;
        }

        public RhetosAspNetServiceCollectionBuilder UseAspNetCoreIdentityUser()
        {
            serviceCollection.AddHttpContextAccessor();
            serviceCollection.AddScoped<IUserInfo, RhetosAspNetCoreIdentityUser>();
            return this;
        }
    }
}
