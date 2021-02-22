using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetDemo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rhetos.Extensions.AspNetCore;
using Rhetos.Processing;
using Rhetos.Processing.DefaultCommands;

namespace WebAppWinAuth.Pages
{
    public class ReadDemoEntitiesModel : PageModel
    {
        public List<DemoEntity> DemoEntityList { get; set; }

        private readonly IRhetosComponent<IProcessingEngine> rhetosProcessingEngine;

        public ReadDemoEntitiesModel(IRhetosComponent<IProcessingEngine> rhetosProcessingEngine)
        {
            this.rhetosProcessingEngine = rhetosProcessingEngine;
        }

        public void OnGet()
        {
            var readCommand = new ReadCommandInfo()
            {
                DataSource = "AspNetDemo.DemoEntity",
                ReadRecords = true
            };
            var result = rhetosProcessingEngine.Value.Execute(new List<ICommandInfo>() {readCommand});
            var readCommandResult = result.CommandResults.FirstOrDefault()?.Data.Value as ReadCommandResult;

            DemoEntityList = readCommandResult?.Records.Cast<DemoEntity>().ToList();
        }
    }
}
