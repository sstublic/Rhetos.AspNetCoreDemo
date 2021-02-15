using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Rhetos.Extensions.RestApi.Utilities;

namespace Rhetos.Extensions.RestApi.Filters
{
    public class ApiExceptionFilter : IActionFilter, IOrderedFilter
    {
        private readonly JsonErrorHandler jsonErrorHandler;
        public int Order { get; } = int.MaxValue - 10;

        public ApiExceptionFilter(JsonErrorHandler jsonErrorHandler)
        {
            this.jsonErrorHandler = jsonErrorHandler;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var (response, statusCode) = jsonErrorHandler.CreateResponseFromException(context.Exception);
                context.Result = new JsonResult(response) { StatusCode = statusCode };
                context.ExceptionHandled = true;
            }
        }
    }
}
