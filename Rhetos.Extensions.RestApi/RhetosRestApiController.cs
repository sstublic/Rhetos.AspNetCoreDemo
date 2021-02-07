using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Dsl;
using Rhetos.Extensions.RestApi.Utilities;
using Rhetos.Processing;
using Rhetos.Processing.DefaultCommands;

namespace Rhetos.Extensions.RestApi
{
    public class RhetosRestApiController<T> : ControllerBase, IRhetosRestApiController<T>, IActionFilter, IOrderedFilter
    {
        private readonly ServiceUtility serviceUtility;

        public RhetosRestApiController(ServiceUtility serviceUtility)
        {
            this.serviceUtility = serviceUtility;
        }

        
        [HttpGet]
        public RecordsAndTotalCountResult<T> Browse(string filter = null, string fparam = null, string genericfilter = null, string filters = null,
            int top = 0, int skip = 0, int page = 0, int psize = 0, string sort = null)
        {
            //var filterType = RestServiceMetadata.GetFilterTypesByDataStructure(typeof(TDataStructure).FullName)
            return serviceUtility.GetData<T>(filter, fparam, genericfilter, filters, null, top, skip, page, psize, sort, true, true);
        }

        [HttpGet]
        [Route("{id}")]
        public T Get(string id)
        {
            return serviceUtility.GetDataById<T>(id);
        }

        [HttpPost]
        public ProcessingResult Post([FromBody]T item)
        {
            return serviceUtility.InsertData<T>(item);
        }

        [HttpGet]
        [Route("TotalCount")]
        public TotalCountResult GetTotalCount(string filter, string fparam, string genericfilter, string filters, string sort)
        {
            var data = serviceUtility.GetData<T>(filter, fparam, genericfilter, filters, null, //RestServiceMetadata.GetFilterTypesByDataStructure(typeof(TDataStructure).FullName),
                0, 0, 0, 0, sort, readRecords: false, readTotalCount: true);
            return new TotalCountResult { TotalCount = data.TotalCount };
        }

        public int Order { get; } = int.MaxValue - 10;

        [NonAction]
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
        
        [NonAction]
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is UserException userException)
            {
                context.Result = new JsonResult(new ErrorResult(userException.Message, userException.SystemMessage )) { StatusCode = StatusCodes.Status400BadRequest };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is ClientException)
            {
                context.Result = new JsonResult(new ErrorResult(context.Exception.Message, null)) { StatusCode = StatusCodes.Status400BadRequest };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is FrameworkException)
            {
                context.Result = new JsonResult(new ErrorResult(null, context.Exception.Message)) { StatusCode = StatusCodes.Status500InternalServerError  };
                context.ExceptionHandled = true;
            }
        }

    }
}
