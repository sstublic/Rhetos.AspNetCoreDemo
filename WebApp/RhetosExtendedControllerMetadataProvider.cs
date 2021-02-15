using System;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensions.RestApi.Controllers;
using Rhetos.Extensions.RestApi.Metadata;

namespace WebApp
{
    public class RhetosExtendedControllerMetadataProvider : IConceptInfoRestMetadataProvider
    {
        public ConceptInfoRestMetadata GetConceptInfoRestMetadata(IConceptInfo conceptInfo)
        {
            if (conceptInfo.GetKeyProperties().Contains("AspNetDemo"))
            {
                var ds = conceptInfo as DataStructureInfo;
                if (ds == null) return null;

                return new ConceptInfoRestMetadata()
                {
                    ControllerType = typeof(RhetosExtendedController<>),
                    ControllerName = $"{ds.Module.Name}.{ds.Name}",
                    RelativeRoute = $"{ds.Module.Name}/{ds.Name}",
                    ApiExplorerGroupName = "rhetos",
                };
            }

            return null;
        }
    }
}
