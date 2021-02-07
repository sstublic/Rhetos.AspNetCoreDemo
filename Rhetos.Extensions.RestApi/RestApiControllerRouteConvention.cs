using System;
using System.Linq;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Rhetos.Dsl.DefaultConcepts;

namespace Rhetos.Extensions.RestApi
{
    public class RestApiControllerRouteConvention : IControllerModelConvention
    {
        private readonly DslModelRestAspect dslModelRestAspect;
        private readonly string baseRoute;

        public RestApiControllerRouteConvention(DslModelRestAspect dslModelRestAspect, string baseRoute)
        {
            this.dslModelRestAspect = dslModelRestAspect;
            this.baseRoute = baseRoute;
        }

        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsClosedTypeOf(typeof(RhetosRestApiController<>)))
            {
                var fromType = controller.ControllerType.GetGenericArguments().Single();
                var info = dslModelRestAspect.GetConceptInfo(fromType);
                var ds = (DataStructureInfo)info;
                var route = $"{baseRoute}/{ds.Module.Name}/{ds.Name}";
                controller.Selectors.Add(new SelectorModel()
                {
                    AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(route))
                });
                controller.ControllerName = ds.Module.Name;
                controller.ApiExplorer.GroupName = "rhetos";
            }
        }
    }
}
