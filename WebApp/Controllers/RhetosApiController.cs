using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common.Queryable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Rhetos.Extensions.AspNetCore;
using Rhetos.Processing;
using Rhetos.Processing.DefaultCommands;
using Rhetos.Utilities;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RhetosApiController : ControllerBase
    {
        private readonly Lazy<IProcessingEngine> processingEngine;
        private readonly Lazy<IUserInfo> rhetosUserInfo;

        public RhetosApiController(Lazy<IProcessingEngine> processingEngine, Lazy<IUserInfo> rhetosUserInfo)
        {
            this.processingEngine = processingEngine;
            this.rhetosUserInfo = rhetosUserInfo;
        }

        [HttpGet]
        [AllowAnonymous]
        public string RequestInfo()
        {
            return $"UserName: '{User?.Identity?.Name}'";
        }

        [HttpGet]
        public string RhetosUserInfo()
        {
            return $"RhetosUserName: '{rhetosUserInfo.Value.UserName}'";
        }

        [HttpGet]
        public string DemoEntity()
        {
            var readCommand = new ReadCommandInfo()
            {
                DataSource = "AspNetDemo.DemoEntity",
                ReadRecords = true,
            };
            var result = processingEngine.Value.Execute(new List<ICommandInfo>() { readCommand });

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        [HttpPost]
        public string DemoEntity(string name)
        {
            if (string.IsNullOrEmpty(name))
                return $"Please specify value for Name property for entity to add.";

            var insertCommand = new SaveEntityCommandInfo()
            {
                Entity = "AspNetDemo.DemoEntity",
                DataToInsert = new[] {new AspNetDemo_DemoEntity() {Name = name}}
            };
            var result = processingEngine.Value.Execute(new List<ICommandInfo>() {insertCommand});

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        [HttpGet]
        [AllowAnonymous]
        public string CommonLog()
        {
            var readCommand = new ReadCommandInfo()
            {
                DataSource = "Common.Log",
                ReadRecords = true,
                OrderByProperties = new []{ new OrderByProperty() { Property = "Created", Descending = true} },
                Top = 100
            };
            var result = processingEngine.Value.Execute(new List<ICommandInfo>() {readCommand});

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        [HttpPost]
        [AllowAnonymous]
        public void SetUserCookie(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                Response.Cookies.Delete(DummyAuthenticationHandler.CookieName);
            else
                Response.Cookies.Append(DummyAuthenticationHandler.CookieName, userName);
        }

        [HttpGet]
        [AllowAnonymous]
        public string GetUserCookie()
        {
            var userName = Request.Cookies[DummyAuthenticationHandler.CookieName];
            return $"UserName cookie value: '{userName}'";
        }
    }
}
