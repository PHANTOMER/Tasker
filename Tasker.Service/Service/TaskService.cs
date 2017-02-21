using System;
using System.ServiceProcess;
using NLog;
using Tasker.Business.Execution;
using Tasker.Business.Jobs;

namespace Tasker.Service.Service
{
    class TaskService : ServiceBase, IServiceBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly DbChangesListener _dbChangesListener;
        private readonly JobLauncher _jobLauncher;
        private readonly TaskManager _taskManager;

        public event EventHandler ServiceStopped;

        public TaskService(
            string serviceName, 
            DbChangesListener dbChangesListener,
            JobLauncher jobLauncher,
            TaskManager taskManager)
        {
            _dbChangesListener = dbChangesListener;
            _jobLauncher = jobLauncher;
            _taskManager = taskManager;

            ServiceName = serviceName;
        }

        #region private methods

        protected override void OnStart(string[] args)
        {
            _logger.Info("Service named '{0}' is starting...", ServiceName);

            try
            {
                _jobLauncher.Start();
                _dbChangesListener.Start();

                _taskManager.GetTasks();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
                throw;
            }

            _logger.Info("Service named '{0}' started!", ServiceName);
        }

        protected override void OnStop()
        {
            _dbChangesListener.Stop();
            _jobLauncher.Stop();

            OnServiceStopped(EventArgs.Empty);
        }

        private void OnServiceStopped(EventArgs e)
        {
            EventHandler stopped = ServiceStopped;
            if (stopped != null) stopped(this, e);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    Stop();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public void Open()
        {
            OnStart(null);
        }

        public void Close()
        {
            OnStop();
        }

        private void InitializeComponent()
        {
            // 
            // TaskService
            // 
            this.ServiceName = "TaskService";

        }
    }
}
