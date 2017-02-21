using System.Data.Entity.Infrastructure;
using Autofac;
using Tasker.Common.Configuration;
using Tasker.DataContext.DependencyListener;
using Tasker.DataContext.Repositories;
using Tasker.DataContext.UnitOfWork;

namespace Tasker.DataContext.DI
{
    public class DataContextModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAdapter<IConfigurationProvider, TaskContextFactory>(
                cfg => new TaskContextFactory(cfg.ConnectionStrings["DefaultConnection"])).As<IDbContextFactory<TaskContext>>();

            builder.RegisterType<TaskRepository>().AsSelf();
            builder.RegisterType<EntityUnitOfWork<TaskContext>>().As<IUnitOfWork>();
            builder.RegisterType<SqlNotificationListenerFactory>().AsSelf();
        }
    }
}
