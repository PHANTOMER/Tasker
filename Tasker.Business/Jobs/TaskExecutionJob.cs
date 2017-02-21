using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.NLog;
using Quartz;
using Tasker.Business.Execution;
using Tasker.Business.Tasks;
using Tasker.DataLayer;

namespace Tasker.Business.Jobs
{
    class TaskExecutionJob : IJob
    {
        private readonly ILogger _logger;
        private readonly TaskManager _taskManager;
        private readonly IEnumerable<ITaskExecutor> _taskExecutors;

        public TaskExecutionJob(
            ILogger logger,
            TaskManager taskManager,
            IEnumerable<ITaskExecutor> taskExecutors)
        {
            _logger = logger;
            _taskManager = taskManager;
            _taskExecutors = taskExecutors;
        }

        public void Execute(IJobExecutionContext context)
        {
            var taskId = context.JobDetail.JobDataMap.Get("Id");
            if (taskId == null)
                throw new JobExecutionException("Parameter \"Id\" was not supplied");

            int taskIdValue = (int) taskId;
            var task = _taskManager.GetTask(taskIdValue);

            if (task != null)
            {
                _logger.Info($"Performing task {taskIdValue} of type {task.TaskType}");

                var executor = _taskExecutors.FirstOrDefault(x => x.CanExecute(task));
                if (executor == null)
                    throw new JobExecutionException($"Task executor for {task.TaskType} was not found");

                bool result = executor.Execute(task);
                task.TaskStatus = result ? TaskStatus.Completed : TaskStatus.Failed;

                _logger.Info($"Task {taskIdValue} completed with status {task.TaskStatus}");

                _taskManager.UpdateTask(task);
            }
        }
    }
}
