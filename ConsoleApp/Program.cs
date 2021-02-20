using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Common.Queryable;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Rhetos;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Extensions.NetCore;
using Rhetos.Logging;
using Rhetos.Processing;
using Rhetos.Processing.DefaultCommands;
using Rhetos.Security;
using Rhetos.Utilities;

namespace ConsoleApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using var rhetosHost = CreateRhetosHostBuilder()
                .Build();

            RunCommandSequence(rhetosHost);
       }

        // Method by convention. Any Rhetos tooling (e.g. dbupdate) will look for this method in current application
        // to create host for all its operations.
        public static IRhetosHostBuilder CreateRhetosHostBuilder()
        {
            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddJsonFile("console-app-settings.json")
                .AddJsonFile("console-app-settings.local.json")
                .Build();

            var rhetosHostBuilder = new RhetosHostBuilder()
                .ConfigureRhetosHostDefaults()
                .ConfigureConfiguration(builder => builder.MapNetCoreConfiguration(configuration))
                .ConfigureContainer(builder =>
                {
                    builder.UseUserInfoProvider(() => new ProcessUserInfo());
                });

            return rhetosHostBuilder;
        }

        private static void RunCommandSequence(RhetosHost rhetosHost)
        {
            using (var scope = rhetosHost.CreateScope())
            {
                var processingEngine = scope.Resolve<IProcessingEngine>();
                var readCommand = new ReadCommandInfo()  { DataSource = "AspNetDemo.DemoEntity",  ReadRecords = true };
                var result = processingEngine.Execute(new List<ICommandInfo>() {readCommand});
                var resultData = result.CommandResults.Single().Data.Value as ReadCommandResult;

                var deleteCommand = new SaveEntityCommandInfo()
                {
                    Entity = "AspNetDemo.DemoEntity",
                    DataToDelete = resultData.Records.Cast<IEntity>().ToArray()
                };
                Console.WriteLine($"Deleting {resultData.Records.Length} records.");
                processingEngine.Execute(new List<ICommandInfo>() {deleteCommand});
                scope.CommitAndClose();
            }

            using (var scope = rhetosHost.CreateScope())
            {
                var processingEngine = scope.Resolve<IProcessingEngine>();
                var readCommand = new ReadCommandInfo() { DataSource = "AspNetDemo.DemoEntity", ReadRecords = true };
                var result = processingEngine.Execute(new List<ICommandInfo>() { readCommand });
                var resultData = result.CommandResults.Single().Data.Value as ReadCommandResult;

                Console.WriteLine($"Reading entities Count={resultData.Records.Length}.");

                var insertCommand = new SaveEntityCommandInfo()
                {
                    Entity = "AspNetDemo.DemoEntity",
                    DataToInsert = new[] { new AspNetDemo_DemoEntity() { Name = "not_saved" } }
                };
                Console.WriteLine($"Inserting entity without committing transaction.");
                processingEngine.Execute(new List<ICommandInfo>() { insertCommand });
            }

            using (var scope = rhetosHost.CreateScope())
            {
                var processingEngine = scope.Resolve<IProcessingEngine>();
                var readCommand = new ReadCommandInfo() {DataSource = "AspNetDemo.DemoEntity", ReadRecords = true};
                var result = processingEngine.Execute(new List<ICommandInfo>() {readCommand});
                var resultData = result.CommandResults.Single().Data.Value as ReadCommandResult;

                Console.WriteLine($"Reading entities Count={resultData.Records.Length}.");

                var insertCommand = new SaveEntityCommandInfo()
                {
                    Entity = "AspNetDemo.DemoEntity",
                    DataToInsert = new[] { new AspNetDemo_DemoEntity() { Name = "InsertedEntityName" } }
                };
                Console.WriteLine($"Inserting entity with commit.");
                processingEngine.Execute(new List<ICommandInfo>() { insertCommand });
                scope.CommitAndClose();
            }

            using (var scope = rhetosHost.CreateScope())
            {
                var processingEngine = scope.Resolve<IProcessingEngine>();
                var readCommand = new ReadCommandInfo() { DataSource = "AspNetDemo.DemoEntity", ReadRecords = true };
                var result = processingEngine.Execute(new List<ICommandInfo>() { readCommand });
                var resultData = result.CommandResults.Single().Data.Value as ReadCommandResult;
                Console.WriteLine($"Reading entities Count={resultData.Records.Length}.");
                Console.WriteLine(JsonConvert.SerializeObject(resultData, Formatting.Indented));
            }
        }
    }
}
