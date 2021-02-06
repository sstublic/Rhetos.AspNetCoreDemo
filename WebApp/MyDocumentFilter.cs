using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Rhetos.Dsl;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApp
{
    public class MyDocumentFilter : IDocumentFilter
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public MyDocumentFilter(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var dslModel = scope.ServiceProvider.GetRequiredService<IDslModel>();
            var pathItem = new OpenApiPathItem();
            pathItem.Description = "PathItem description";
            var operation = new OpenApiOperation()
            {
                Description = $"operation description: {dslModel.Concepts.Count()}",
                OperationId = "opId",
            };
            operation.Tags.Add(new OpenApiTag() { Name = "TagName", Description = "Tag Description"});
            pathItem.Operations.Add(OperationType.Post, operation);
            swaggerDoc.Paths.Add("/pero/pff", pathItem);
        }
    }
}
