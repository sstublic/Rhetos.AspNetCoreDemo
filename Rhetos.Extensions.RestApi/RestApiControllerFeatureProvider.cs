using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Rhetos.Extensions.RestApi
{
    public class RestApiControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly DslModelRestAspect dslModelRestAspect;

        public RestApiControllerFeatureProvider(DslModelRestAspect dslModelRestAspect)
        {
            this.dslModelRestAspect = dslModelRestAspect;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var supportedType in dslModelRestAspect.RestSupportedTypes)
            {
                feature.Controllers.Add(typeof(RhetosRestApiController<>).MakeGenericType(supportedType).GetTypeInfo());
            }
        }
    }
}
