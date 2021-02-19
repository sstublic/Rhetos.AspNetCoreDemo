using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rhetos;
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
        private static readonly IConceptInfoRestMetadataProvider[] _defaultMetadataProviders =
        {
            new ActionInfoRestMetadataProvider(),
            new ReportDataInfoRestMetadataProvider(),
            new DataStructureInfoRestMetadataProvider(),
        };

        public static RhetosAspNetServiceCollectionBuilder AddRestApi(this RhetosAspNetServiceCollectionBuilder builder, 
            Action<RestApiOptions> configureOptions, Action<ControllerRestInfoRepository> onControllerRestInfoCreated = null)
        {
            var options = new RestApiOptions()
            {
                BaseRoute = "RhetosRestApi",
                ConceptInfoRestMetadataProviders = new List<IConceptInfoRestMetadataProvider>(_defaultMetadataProviders)
            };

            configureOptions?.Invoke(options);

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

            var controllerRepository = CreateControllerRestInfoRepository(builder.RhetosHost, options);
            onControllerRestInfoCreated?.Invoke(controllerRepository);

            builder.Services
                .AddControllers(o =>
                    o.Conventions.Add(new RestApiControllerRouteConvention(options, controllerRepository)))
                .ConfigureApplicationPartManager(p =>
                    p.FeatureProviders.Add(new RestApiControllerFeatureProvider(controllerRepository)));

            return builder;
        }

        private static ControllerRestInfoRepository CreateControllerRestInfoRepository(RhetosHost rhetosHost, RestApiOptions options)
        {
            var controllerRepository = new ControllerRestInfoRepository();
            foreach (var conceptInfoRestMetadataProvider in options.ConceptInfoRestMetadataProviders)
            {
                var metadataFromProvider = conceptInfoRestMetadataProvider.GetConceptInfoRestMetadata(rhetosHost);
                foreach (var metadataItem in metadataFromProvider)
                    controllerRepository.ControllerConceptInfo.Add(metadataItem.ControllerType, metadataItem);
            }

            // transform all group names
            if (options.GroupNameMapper != null)
            {
                foreach (var restMetadata in controllerRepository.ControllerConceptInfo.Values)
                    restMetadata.ApiExplorerGroupName = options.GroupNameMapper.Invoke(restMetadata.ConceptInfo, restMetadata.ApiExplorerGroupName);
            }

            return controllerRepository;
        }
    }
}
