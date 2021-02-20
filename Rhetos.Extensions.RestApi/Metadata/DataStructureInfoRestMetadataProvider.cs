using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Rhetos.Dom;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensions.RestApi.Controllers;

namespace Rhetos.Extensions.RestApi.Metadata
{
    public class DataStructureInfoRestMetadataProvider : IConceptInfoRestMetadataProvider
    {
        public IEnumerable<ConceptInfoRestMetadata> GetConceptInfoRestMetadata(RhetosHost rhetosHost)
        {
            var dslModel = rhetosHost.GetRootContainer().Resolve<IDslModel>();
            var domainObjectModel = rhetosHost.GetRootContainer().Resolve<IDomainObjectModel>();
            var dataStructureReadParameters = rhetosHost.GetRootContainer().Resolve<IDataStructureReadParameters>();

            Tuple<string, Type>[] FromDataStructureInfo(DataStructureInfo dataStructureInfo)
            {
                return dataStructureReadParameters.GetReadParameters(dataStructureInfo.FullName, true)
                    .Select(a => Tuple.Create(a.Name, a.Type))
                    .ToArray();
            }

            var restMetadata = dslModel
                .FindByType<DataStructureInfo>()
                .Where(IsDataStructureTypeSupported)
                .Select(dataStructureInfo => new DataStructureInfoMetadata(FromDataStructureInfo(dataStructureInfo))
                {
                    ControllerType = typeof(DataApiController<>).MakeGenericType(domainObjectModel.GetType($"{dataStructureInfo.FullName}")),
                    ControllerName = $"{dataStructureInfo.Module.Name}.{dataStructureInfo.Name}",
                    RelativeRoute = $"{dataStructureInfo.Module.Name}/{dataStructureInfo.Name}",
                    ApiExplorerGroupName = dataStructureInfo.Module.Name,
                });

            return restMetadata;
        }

        private static bool IsDataStructureTypeSupported(DataStructureInfo conceptInfo)
        {
            return conceptInfo is IOrmDataStructure
                   || conceptInfo is BrowseDataStructureInfo
                   || conceptInfo is QueryableExtensionInfo
                   || conceptInfo is ComputedInfo;
        }

    }
}
