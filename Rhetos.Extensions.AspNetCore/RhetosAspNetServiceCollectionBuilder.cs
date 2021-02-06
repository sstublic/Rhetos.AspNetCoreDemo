using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Rhetos.Utilities;

namespace Rhetos.Extensions.AspNetCore
{
    public class RhetosAspNetServiceCollectionBuilder
    {
        public IServiceCollection Services { get; }
        public RhetosAspNetServiceCollectionBuilder(IServiceCollection serviceCollection)
        {
            Services = serviceCollection;
        }

        public RhetosAspNetServiceCollectionBuilder ExposeRhetosComponent<T>() where T : class
        {
            if (typeof(T) == typeof(IUserInfo))
                throw new InvalidOperationException($"Adding explicit IUserInfo registration would result in circular dependency resolution."
                                                    + " Register IUserInfo implementation via AddRhetosUserInfo<T>().");

            Services.AddScoped(serviceProvider => serviceProvider
                .GetRequiredService<RhetosScopeServiceProvider>()
                .Resolve<T>());

            return this;
        }

        public RhetosAspNetServiceCollectionBuilder UseAspNetCoreIdentityUser()
        {
            Services.AddHttpContextAccessor();
            Services.AddScoped<IUserInfo, RhetosAspNetCoreIdentityUser>();
            return this;
        }
    }
}
