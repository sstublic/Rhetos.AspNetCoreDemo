using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rhetos.Extensions.AspNetCore;
using Rhetos.Extensions.RestApi;
using Rhetos.Extensions.RestApi.Utilities;
using Rhetos.Processing;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RhetosRestApiServiceCollectionExtensions
    {
        public static RhetosAspNetServiceCollectionBuilder AddRestApi(this RhetosAspNetServiceCollectionBuilder builder, string baseRoute)
        {
            builder.ExposeRhetosComponent<IProcessingEngine>();
            builder.Services.AddSingleton<QueryParameters>();
            builder.Services.AddScoped<ServiceUtility>();
         
            var dslModelRestAspect = new DslModelRestAspect(builder.RhetosHost);
            builder.Services.AddSingleton(dslModelRestAspect);

            builder.Services
                .AddControllers(o => o.Conventions.Add(new RestApiControllerRouteConvention(dslModelRestAspect, baseRoute)))
                .ConfigureApplicationPartManager(p => p.FeatureProviders.Add(new RestApiControllerFeatureProvider(dslModelRestAspect)));

            return builder;
        }
    }
}
