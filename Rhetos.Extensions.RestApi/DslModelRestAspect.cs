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
            domainObjectModel = new Lazy<IDomainObjectModel>(() => ResolveFromHost<IDomainObjectModel>(rhetosHost));

            typeConceptInfo = new Lazy<Dictionary<Type, IConceptInfo>>(() =>
            {
                var dslModel = ResolveFromHost<IDslModel>(rhetosHost);
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

        public IConceptInfo GetConceptInfo(Type type)
        {
            return typeConceptInfo.Value[type];
        }

        public static bool IsTypeSupported(DataStructureInfo conceptInfo)
        {
            return conceptInfo is IOrmDataStructure
                   || conceptInfo is BrowseDataStructureInfo
                   || conceptInfo is QueryableExtensionInfo
                   || conceptInfo is ComputedInfo;
        }

        public static T ResolveFromHost<T>(RhetosHost rhetosHost)
        {
            var container = typeof(RhetosHost)
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(a => a.Name == "Container")
                .GetValue(rhetosHost) as IContainer;
            return container.Resolve<T>();
        }
    }
}
