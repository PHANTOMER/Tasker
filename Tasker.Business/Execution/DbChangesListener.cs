using System;
using Tasker.DataContext.DependencyListener;
using Task = Tasker.DataLayer.Task;

namespace Tasker.Business.Execution
{
    public class DbChangesListener
    {
        private readonly SqlNotificationListenerFactory _sqlNotificationListenerFactory;
        private readonly TaskManager _taskManager;
        private SqlNotificationListener<Task> _sqlNotificationListener;

        public DbChangesListener(SqlNotificationListenerFactory sqlNotificationListenerFactory, TaskManager taskManager)
        {
            _sqlNotificationListenerFactory = sqlNotificationListenerFactory;
            _taskManager = taskManager;
        }

        public void Start()
        {
            _sqlNotificationListener = _sqlNotificationListenerFactory.Create();

            _sqlNotificationListener.OnChanged += SqlNotificationListenerOnOnChanged;

            _sqlNotificationListener.StartMonitor();

            _sqlNotificationListener.RegisterSqlDependency();
        }

        private void SqlNotificationListenerOnOnChanged(object sender, EventArgs eventArgs)
        {
            _taskManager.GetTasks();
        }

        public void Stop()
        {
            if (_sqlNotificationListener != null)
            {
                _sqlNotificationListener.OnChanged -= SqlNotificationListenerOnOnChanged;
                _sqlNotificationListener.StopMonitor();
                _sqlNotificationListener = null;
            }
        }
    }
}
