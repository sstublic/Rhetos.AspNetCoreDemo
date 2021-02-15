using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.StaticFiles;
using Rhetos.Dom;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensions.RestApi.Controllers;
using Rhetos.Extensions.RestApi.Metadata;

namespace Rhetos.Extensions.RestApi
{
    public class RestApiControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly DslModelRestAspect dslModelRestAspect;
        private readonly RhetosHost rhetosHost;
        private readonly IConceptInfoRestMetadataProvider[] conceptInfoRestMetadataProviders;
        private readonly ControllerRestInfoRepository controllerRestInfoRepository;

        public RestApiControllerFeatureProvider(DslModelRestAspect dslModelRestAspect, RhetosHost rhetosHost,
            IConceptInfoRestMetadataProvider[] conceptInfoRestMetadataProviders, ControllerRestInfoRepository controllerRestInfoRepository)
        {
            this.dslModelRestAspect = dslModelRestAspect;
            this.rhetosHost = rhetosHost;
            this.conceptInfoRestMetadataProviders = conceptInfoRestMetadataProviders;
            this.controllerRestInfoRepository = controllerRestInfoRepository;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var domainObjectModel = DslModelRestAspect.ResolveFromHost<IDomainObjectModel>(rhetosHost);

            foreach (var domainType in domainObjectModel.GetTypes())
            {
                var conceptInfo = dslModelRestAspect.GetConceptInfo(domainType);
                if (conceptInfo == null)
                    continue;

                var conceptRestMetadata = conceptInfoRestMetadataProviders
                    .Select(a => a.GetConceptInfoRestMetadata(conceptInfo))
                    .Where(a => a != null)
                    .ToList();

                if (conceptRestMetadata.Count == 0)
                    continue;

                foreach (var conceptMetadata in conceptRestMetadata)
                {
                    var controllerType = conceptMetadata.ControllerType.MakeGenericType(domainType);
                    feature.Controllers.Add(controllerType.GetTypeInfo());
                    controllerRestInfoRepository.ControllerConceptInfo[controllerType] = conceptMetadata;
                }
            }
        }
    }
}
