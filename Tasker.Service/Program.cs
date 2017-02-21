using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Autofac;
using Autofac.Extras.NLog;
using Tasker.Business.DI;
using Tasker.Business.Execution;
using Tasker.Business.Jobs;
using Tasker.Common.Autofac;
using Tasker.Common.Configuration;
using Tasker.Common.Extensions;
using Tasker.Service.Service;

namespace Tasker.Service
{
    class Program
    {
        [STAThread]

        private static void Main()
        {
            bool debug = IsDebugMode();
            if (debug)
            {
                Debugger.Launch();
            }
            
            AutofacCore.Init(new BusinessModule(), new NLogModule());

            IConfigurationProvider configuration = AutofacCore.Core.Resolve<IConfigurationProvider>();
            SetWorkerId(configuration);

            using (TaskService service = GetService(configuration))
            {
                if (IsConsoleMode(configuration))
                {
                    service.Open();

                    ConsoleKeyInfo info = Console.ReadKey(true);
                    while (info.Key != ConsoleKey.Enter)
                    {
                        info = Console.ReadKey(true);
                    }

                    service.Close();
                }
                else
                {
                    ServiceBase.Run(service);
                }
            }
        }

        private static bool IsDebugMode()
        {
            var args = Environment.GetCommandLineArgs();
            return (args.Length > 0 && args.Any(x => x.ToLower() == "debug"));
        }

        private static bool IsConsoleMode(IConfigurationProvider configuration)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 0 && args.Any(x => x.ToLower() == "service"))
                return false;

            return configuration.AppSettings["IsConsole"].ToBoolean();
        }

        private static TaskService GetService(IConfigurationProvider configuration)
        {
            var serviceName = GetServiceNameFromConfiguration(configuration);

            return new TaskService(serviceName, 
                AutofacCore.Core.Resolve<DbChangesListener>(), 
                AutofacCore.Core.Resolve<JobLauncher>(),
                AutofacCore.Core.Resolve<TaskManager>());
        }

        private static void SetWorkerId(IConfigurationProvider configuration)
        {
            string workerId = configuration.AppSettings["WorkerId"];
            if (workerId.IsNullOrEmpty())
            {
                configuration.UpdateSettings(new Dictionary<string, object>
                {
                    {
                        "WorkerId", Guid.NewGuid().ToString("n")
                    }
                });
            }
        }

        private static string GetServiceNameFromConfiguration(IConfigurationProvider configuration)
        {
            // READ THIS: Get the service name from App.Config
            var serviceName = configuration.AppSettings["ServiceName"];

            if (string.IsNullOrEmpty(serviceName))
                serviceName = "NotSpecified";

            return serviceName;
        }
    }
}
