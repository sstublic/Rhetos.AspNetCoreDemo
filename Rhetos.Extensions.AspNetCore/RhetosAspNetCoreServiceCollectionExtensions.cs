using System;
using Rhetos;
using Rhetos.Extensions.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RhetosAspNetCoreServiceCollectionExtensions
    {
        public static RhetosAspNetServiceCollectionBuilder AddRhetos(this IServiceCollection serviceCollection, IRhetosHostBuilder rhetosHostBuilder,
            Action<IRhetosHostBuilder> configureRhetosHost = null)
        {
            configureRhetosHost?.Invoke(rhetosHostBuilder);
            var rhetosHost = rhetosHostBuilder.Build();
                
            serviceCollection.AddSingleton(rhetosHost);
            serviceCollection.AddScoped<RhetosScopeServiceProvider>();

            return new RhetosAspNetServiceCollectionBuilder(serviceCollection, rhetosHost);
        }
    }
}
