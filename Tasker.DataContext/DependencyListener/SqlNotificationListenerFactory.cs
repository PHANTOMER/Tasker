using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using Tasker.DataLayer;
using Task = Tasker.DataLayer.Task;

namespace Tasker.DataContext.DependencyListener
{
    public class SqlNotificationListenerFactory
    {
        private readonly IDbContextFactory<TaskContext> _contextFactory;

        public SqlNotificationListenerFactory(IDbContextFactory<TaskContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public SqlNotificationListener<Task> Create()
        {
            TaskContext context = _contextFactory.Create();

            IQueryable query = context.Set<Task>().Where(x => x.TaskStatus == TaskStatus.Created); 

            if (!CanRequestNotifications())
                throw new Exception("Can't enable SqlDependency notifications");

            return new SqlNotificationListener<Task>(context, query);
        }

        private bool CanRequestNotifications()
        {
            // In order to use the callback feature of the
            // SqlDependency, the application must have
            // the SqlClientPermission permission.
            try
            {
                SqlClientPermission perm =
                    new SqlClientPermission(
                    PermissionState.Unrestricted);

                perm.Demand();

                return true;
            }
            catch (SecurityException se)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
