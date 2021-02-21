﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Common.Queryable;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Rhetos.Extensions.AspNetCore;
using Rhetos.Processing;
using Rhetos.Processing.DefaultCommands;
using Rhetos.Utilities;

namespace WebApp.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("[controller]/[action]")]
    public class RhetosApiController : ControllerBase
    {
        private readonly IRhetosComponent<IProcessingEngine> rhetosProcessingEngine;
        private readonly IUserInfo rhetosUserInfo;

        public RhetosApiController(IRhetosComponent<IProcessingEngine> rhetosProcessingEngine, IUserInfo rhetosUserInfo)
        {
            this.rhetosProcessingEngine = rhetosProcessingEngine;
            this.rhetosUserInfo = rhetosUserInfo;
        }

        [HttpGet]
        public string RhetosUserInfo()
        {
            return $"RhetosUserName: '{rhetosUserInfo.UserName}'";
        }

        [HttpGet]
        public string DemoEntity()
        {
            var readCommand = new ReadCommandInfo()
            {
                DataSource = "AspNetDemo.DemoEntity",
                ReadRecords = true,
            };
            var result = rhetosProcessingEngine.Value.Execute(new List<ICommandInfo>() { readCommand });
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
            var result = rhetosProcessingEngine.Value.Execute(new List<ICommandInfo>() {insertCommand});

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        [HttpGet]
        public string CommonLog()
        {
            var readCommand = new ReadCommandInfo()
            {
                DataSource = "Common.Log",
                ReadRecords = true,
                OrderByProperties = new []{ new OrderByProperty() { Property = "Created", Descending = true} },
                Top = 100
            };
            var result = rhetosProcessingEngine.Value.Execute(new List<ICommandInfo>() {readCommand});

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }
    }
}
