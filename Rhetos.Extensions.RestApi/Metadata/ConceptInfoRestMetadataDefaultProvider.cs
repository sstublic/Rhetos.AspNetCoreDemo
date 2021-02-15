using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensions.RestApi.Controllers;

namespace Rhetos.Extensions.RestApi.Metadata
{
    public class ConceptInfoRestMetadataDefaultProvider : IConceptInfoRestMetadataProvider
    {
        public ConceptInfoRestMetadata GetConceptInfoRestMetadata(IConceptInfo conceptInfo)
        {
            if (conceptInfo is ActionInfo actionInfo)
            {
                return new ConceptInfoRestMetadata()
                {
                    ControllerType = typeof(ActionApiController<>),
                    ControllerName = $"{actionInfo.Module.Name}.{actionInfo.Name}",
                    RelativeRoute = $"{actionInfo.Module.Name}/{actionInfo.Name}",
                    ApiExplorerGroupName = "rhetos",
                };
            }

            if (conceptInfo is ReportDataInfo reportDataInfo)
            {
                return new ConceptInfoRestMetadata()
                {
                    ControllerType = typeof(ReportApiController<>),
                    ControllerName = $"{reportDataInfo.Module.Name}.{reportDataInfo.Name}",
                    RelativeRoute = $"{reportDataInfo.Module.Name}/{reportDataInfo.Name}",
                    ApiExplorerGroupName = "rhetos",
                };
            }

            if (conceptInfo is DataStructureInfo dataStructureConceptInfo
                && IsDataStructureTypeSupported(dataStructureConceptInfo))
            {
                return new ConceptInfoRestMetadata()
                {
                    ControllerType = typeof(DataApiController<>),
                    ControllerName = $"{dataStructureConceptInfo.Module.Name}.{dataStructureConceptInfo.Name}",
                    RelativeRoute = $"{dataStructureConceptInfo.Module.Name}/{dataStructureConceptInfo.Name}",
                    ApiExplorerGroupName = "rhetos",
                };
            }

            return null;
        }

        public static bool IsDataStructureTypeSupported(DataStructureInfo conceptInfo)
        {
            return conceptInfo is IOrmDataStructure
                   || conceptInfo is BrowseDataStructureInfo
                   || conceptInfo is QueryableExtensionInfo
                   || conceptInfo is ComputedInfo;
        }
    }
}
