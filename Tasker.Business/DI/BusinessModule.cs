using Autofac;
using Tasker.Business.Execution;
using Tasker.Business.Jobs;
using Tasker.Business.Tasks;
using Tasker.Business.Utils;
using Tasker.Common.Configuration;
using Tasker.Common.Extensions;
using Tasker.DataContext.DI;

namespace Tasker.Business.DI
{
    public class BusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AppConfigurationProvider>().As<IConfigurationProvider>();

            builder.RegisterAdapter<IConfigurationProvider, TaskPool>(
                cfg => new TaskPool(cfg.AppSettings["MaxConcurrentTasks"].ToInt())).SingleInstance();

            builder.RegisterType<TaskManager>().AsSelf();
            builder.RegisterType<JobFactory>().AsSelf();
            builder.RegisterType<JobLauncher>().AsSelf().SingleInstance();
            builder.RegisterType<TaskExecutionJob>().AsSelf();

            builder.RegisterType<SchedulerFactory>().As<ISchedulerFactory>();
            builder.RegisterType<DbChangesListener>().AsSelf().SingleInstance();

            builder.RegisterModule(new DataContextModule());
            builder.RegisterType<CreateFileTaskExecutor>().As<ITaskExecutor>();
            builder.RegisterType<SendEmailTaskExecutor>().As<ITaskExecutor>();
            builder.RegisterType<EmailUtils>().AsSelf();
        }
    }
}
