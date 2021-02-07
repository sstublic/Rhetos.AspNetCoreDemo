using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Dsl;
using Rhetos.Processing;
using Rhetos.Processing.DefaultCommands;

namespace Rhetos.Extensions.RestApi
{
    /*
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("MyTest/{module}/{entity}")]
    public class MyApi : ControllerBase, IActionFilter
    {
        private readonly IProcessingEngine processingEngine;
        private readonly DslModelRestAspect dslModelRestAspect;

        private IConceptInfo actionConceptInfo;

        public MyApi(IProcessingEngine processingEngine, DslModelRestAspect dslModelRestAspect)
        {
            this.processingEngine = processingEngine;
            this.dslModelRestAspect = dslModelRestAspect;
        }

        [HttpGet]
        public IActionResult Get()
        {
            Console.WriteLine($"GET {actionConceptInfo.GetKeyProperties()}");

            var readCommand = new ReadCommandInfo()
            {
                DataSource = actionConceptInfo.GetKeyProperties(),
                ReadRecords = true,
                //Filters = new [] { new FilterCriteria("ID", "equals", id), }
            };

            var result = processingEngine.Execute(new List<ICommandInfo>() { readCommand });
            if (!result.Success)
                return BadRequest(result.SystemMessage ?? result.UserMessage);

            var readCommandResult = (ReadCommandResult)result.CommandResults[0].Data.Value;
            if (readCommandResult.Records.Length == 0)
                return NotFound();

            return new JsonResult(readCommandResult.Records);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(string id)
        {
            Console.WriteLine($"GET {actionConceptInfo.GetKeyProperties()}/{id}");

            var readCommand = new ReadCommandInfo()
            {
                DataSource = actionConceptInfo.GetKeyProperties(),
                ReadRecords = true,
                Filters = new [] { new FilterCriteria("ID", "equals", id), }
            };

            var result = processingEngine.Execute(new List<ICommandInfo>() { readCommand });
            if (!result.Success)
                return BadRequest(result.SystemMessage ?? result.UserMessage);

            var readCommandResult = (ReadCommandResult)result.CommandResults[0].Data.Value;
            if (readCommandResult.Records.Length == 0)
                return NotFound();

            return new JsonResult(readCommandResult.Records);
        }


        [NonAction]
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var module = context.RouteData.Values["module"] as string;
            var entity = context.RouteData.Values["entity"] as string;
            actionConceptInfo = dslModelRestAspect.ResolveValidConceptInfo(module, entity);

            if (actionConceptInfo == null)
            {
                context.Result = new NotFoundResult();
                Console.WriteLine($"Invalid entity '{module}.{entity}'.");
            }
        }

        [NonAction]
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }
    }
    */
}
