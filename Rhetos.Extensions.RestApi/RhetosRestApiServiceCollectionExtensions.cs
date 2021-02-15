using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rhetos.Extensions.AspNetCore;
using Rhetos.Extensions.RestApi;
using Rhetos.Extensions.RestApi.Filters;
using Rhetos.Extensions.RestApi.Metadata;
using Rhetos.Extensions.RestApi.Utilities;
using Rhetos.Persistence;
using Rhetos.Processing;
using Rhetos.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RhetosRestApiServiceCollectionExtensions
    {
        public static RhetosAspNetServiceCollectionBuilder AddRestApi(this RhetosAspNetServiceCollectionBuilder builder, string baseRoute,
            IEnumerable<IConceptInfoRestMetadataProvider> conceptInfoRestMetadataProviders = null)
        {
            var restMetadataProviders = conceptInfoRestMetadataProviders?.ToArray() ?? new[] {new ConceptInfoRestMetadataDefaultProvider()};

            builder.ExposeRhetosComponent<IProcessingEngine>();
            builder.ExposeRhetosComponent<IPersistenceTransaction>();
            builder.ExposeRhetosComponent<ILocalizer>();
            builder.Services.AddSingleton<QueryParameters>();
            builder.Services.AddScoped<ServiceUtility>();
            builder.Services.AddScoped<JsonErrorHandler>();
            
            builder.Services.AddScoped<ApiExceptionFilter>();
            builder.Services.AddScoped<ApiCommitOnSuccessFilter>();

            var dslModelRestAspect = new DslModelRestAspect(builder.RhetosHost);
            builder.Services.AddSingleton(dslModelRestAspect);

            // create and share instance of repository
            var controllerRepository = new ControllerRestInfoRepository();

            builder.Services
                .AddControllers(o =>
                {
                    o.Conventions.Add(new RestApiControllerRouteConvention(baseRoute, controllerRepository));
                })
                .ConfigureApplicationPartManager(p =>
                {
                    p.FeatureProviders.Add(new RestApiControllerFeatureProvider(dslModelRestAspect, builder.RhetosHost, restMetadataProviders, controllerRepository));
                });

            return builder;
        }
    }
}
