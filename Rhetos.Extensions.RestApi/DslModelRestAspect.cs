using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;

namespace Rhetos.Extensions.RestApi
{
    public class DslModelRestAspect
    {
        private readonly Lazy<IDslModel> dslModel;
        private readonly Lazy<Dictionary<string, IConceptInfo>> conceptsByKey;
        public DslModelRestAspect(RhetosHost rhetosHost)
        {
            dslModel = new Lazy<IDslModel>(() => ResolveDslModel(rhetosHost));
            conceptsByKey = new Lazy<Dictionary<string, IConceptInfo>>(() =>
            {
                return dslModel.Value.Concepts
                    .Where(a => IsTypeSupported(a as DataStructureInfo))
                    .ToDictionary(a => a.GetKeyProperties(), a => a, StringComparer.InvariantCultureIgnoreCase);
            });
        }

        public IConceptInfo ResolveValidConceptInfo(string module, string entityName)
        {
            if (!conceptsByKey.Value.TryGetValue($"{module}.{entityName}", out var conceptInfo))
                return null;

            if (!IsTypeSupported(conceptInfo as DataStructureInfo))
                return null;

            return conceptInfo;
        }

        private static bool IsTypeSupported(DataStructureInfo conceptInfo)
        {
            return conceptInfo is IOrmDataStructure
                   || conceptInfo is BrowseDataStructureInfo
                   || conceptInfo is QueryableExtensionInfo
                   || conceptInfo is ComputedInfo;
        }

        private IDslModel ResolveDslModel(RhetosHost rhetosHost)
        {
            var container = typeof(RhetosHost)
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(a => a.Name == "Container")
                .GetValue(rhetosHost) as IContainer;
            return container.Resolve<IDslModel>();
        }
    }
}
