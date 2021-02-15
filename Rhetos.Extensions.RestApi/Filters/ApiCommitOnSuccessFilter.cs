using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Rhetos.Persistence;

namespace Rhetos.Extensions.RestApi.Filters
{
    public class ApiCommitOnSuccessFilter : IActionFilter, IOrderedFilter
    {
        private readonly IPersistenceTransaction persistenceTransaction;
        public int Order { get; } = int.MaxValue - 20;

        public ApiCommitOnSuccessFilter(IPersistenceTransaction persistenceTransaction)
        {
            this.persistenceTransaction = persistenceTransaction;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Response.StatusCode == 200 && context.Exception == null)
            {
                persistenceTransaction.CommitChanges();
                persistenceTransaction.Dispose();
            }
        }
    }
}
