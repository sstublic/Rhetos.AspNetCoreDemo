using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Queryable;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Rhetos.Dom.DefaultConcepts;
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
        private readonly IRhetosComponent<ExecutionContext> rhetosExecutionContext;

        public RhetosApiController(IRhetosComponent<IProcessingEngine> rhetosProcessingEngine, IUserInfo rhetosUserInfo,
            IRhetosComponent<ExecutionContext> rhetosExecutionContext)
        {
            this.rhetosProcessingEngine = rhetosProcessingEngine;
            this.rhetosUserInfo = rhetosUserInfo;
            this.rhetosExecutionContext = rhetosExecutionContext;
        }

        [HttpPost]
        public void InitUser(string userName)
        {
            var common = rhetosExecutionContext.Value.Repository.Common;
            var principal = new Common.Principal() { Name = userName, ID = Guid.NewGuid() };
            common.Principal.Insert(new [] {principal});
            rhetosExecutionContext.Value.PersistenceTransaction.CommitChanges();
        }

        [HttpPost]
        public void AddClaim(string userName, string claimResource, string claimRight)
        {
            var common = rhetosExecutionContext.Value.Repository.Common;
            var user = common.Principal.Query().Single(a => a.Name == userName);
            var claim = common.Claim.Query().Single(a => a.ClaimResource == claimResource && a.ClaimRight == claimRight);
            var permission = new PrincipalPermission() { ID = Guid.NewGuid(), IsAuthorized = true, ClaimID = claim.ID, PrincipalID = user.ID};
            common.PrincipalPermission.Insert(new [] {permission});
            rhetosExecutionContext.Value.PersistenceTransaction.CommitChanges();
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
