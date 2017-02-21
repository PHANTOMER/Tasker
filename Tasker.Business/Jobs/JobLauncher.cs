using System;
using Autofac.Extras.NLog;
using Quartz;
using Tasker.DataLayer;

namespace Tasker.Business.Jobs
{
    public class JobLauncher
    {
        private readonly ILogger _logger;

        private readonly ISchedulerFactory _schedulerFactory;
        private readonly JobFactory _jobFactory;
        private IScheduler _scheduler;

        public JobLauncher(ILogger logger, ISchedulerFactory schedulerFactory, JobFactory jobFactory)
        {
            _logger = logger;
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
        }

        public void Start()
        {
            if (_scheduler != null)
                throw new Exception("Scheduler was already started");

            _logger.Trace("Starting scheduler...");

            _scheduler = _schedulerFactory.Create();

            if (!_scheduler.IsStarted)
                _scheduler.Start();
        }

        public void Stop()
        {
            if (_scheduler.IsShutdown)
                return;

            _logger.Trace("Shutting down all jobs...");
            _scheduler.Clear();
            _scheduler.Shutdown(false);
        }

        public void AddNewTaskJob(Task task)
        {
            _jobFactory.CreateTaskExecutionJob(_scheduler, new TaskJobData()
            {
                StartAt = task.StartsAfter,
                TaskId = task.Id
            });
        }
    }
}
