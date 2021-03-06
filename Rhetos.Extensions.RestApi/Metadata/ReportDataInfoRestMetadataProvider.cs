﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Rhetos.Dom;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensions.RestApi.Controllers;

namespace Rhetos.Extensions.RestApi.Metadata
{
    public class ReportDataInfoRestMetadataProvider : IConceptInfoRestMetadataProvider
    {
        public IEnumerable<ConceptInfoRestMetadata> GetConceptInfoRestMetadata(RhetosHost rhetosHost)
        {
            var dslModel = rhetosHost.GetRootContainer().Resolve<IDslModel>();
            var domainObjectModel = rhetosHost.GetRootContainer().Resolve<IDomainObjectModel>();

            var restMetadata = dslModel
                .FindByType<ReportDataInfo>()
                .Select(reportDataInfo => new ConceptInfoRestMetadata()
                {
                    ControllerType = typeof(ReportApiController<>).MakeGenericType(domainObjectModel.GetType($"{reportDataInfo.FullName}")),
                    ControllerName = $"{reportDataInfo.Module.Name}.{reportDataInfo.Name}",
                    RelativeRoute = $"{reportDataInfo.Module.Name}/{reportDataInfo.Name}",
                    ApiExplorerGroupName = reportDataInfo.Module.Name
                });

            return restMetadata;
        }
    }
}
