using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApp
{
    public class CustomExtendedController : ControllerBase
    {
        [HttpGet]
        [ApiExplorerSettings(GroupName = "rhetos")]
        [Route("RhetosRestApiTest/AspNetDemo/DemoEntity/Execute")]
        public string Extension()
        {
            return "sasa";
        }
    }
}
