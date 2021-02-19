using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Rhetos.Dom;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;

namespace Rhetos.Extensions.RestApi
{
    public class DslModelRestAspect
    {
        public List<Type> RestSupportedTypes => restSupportedTypes.Value;
        public IDomainObjectModel DomainObjectModel => domainObjectModel.Value;

        private readonly Lazy<Dictionary<Type, IConceptInfo>> typeConceptInfo;
        private readonly Lazy<List<Type>> restSupportedTypes;
        private readonly Lazy<IDomainObjectModel> domainObjectModel;

        public DslModelRestAspect(RhetosHost rhetosHost)
        {
            domainObjectModel = new Lazy<IDomainObjectModel>(() =>  rhetosHost.GetRootContainer().Resolve<IDomainObjectModel>());
            typeConceptInfo = new Lazy<Dictionary<Type, IConceptInfo>>(() =>
            {
                var dslModel = rhetosHost.GetRootContainer().Resolve<IDslModel>();
                var assembly = domainObjectModel.Value.Assemblies.Single();

                return dslModel.Concepts
                    .Where(a => a is DataStructureInfo)
                    .ToDictionary(a => assembly.GetType(a.GetKeyProperties()), a => a);
            });

            restSupportedTypes = new Lazy<List<Type>>(() =>
            {
                return typeConceptInfo.Value
                    .Where(a => IsTypeSupported(a.Value as DataStructureInfo))
                    .Select(a => a.Key)
                    .ToList();
            });
        }

        // TODO: Temporary solution
        public Tuple<string, Type>[] GetFilterTypes()
        {
            return typeConceptInfo.Value
                .Select(a => Tuple.Create(a.Value.GetKeyProperties(), a.Key))
                .ToArray();
        }

        public IConceptInfo GetConceptInfo(Type type)
        {
            return typeConceptInfo.Value.TryGetValue(type, out var conceptInfo)
                ? conceptInfo
                : null;
        }

        public static bool IsTypeSupported(DataStructureInfo conceptInfo)
        {
            return conceptInfo is IOrmDataStructure
                   || conceptInfo is BrowseDataStructureInfo
                   || conceptInfo is QueryableExtensionInfo
                   || conceptInfo is ComputedInfo;
        }
    }
}
