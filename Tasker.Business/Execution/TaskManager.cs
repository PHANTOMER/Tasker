using System.Linq;
using Tasker.Business.Jobs;
using Tasker.Business.Tasks;
using Tasker.Common.Configuration;
using Tasker.DataContext.Repositories;
using Tasker.DataLayer;

namespace Tasker.Business.Execution
{
    public class TaskManager
    {
        private readonly TaskRepository _taskRepository;
        private readonly TaskPool _taskPool;
        private readonly JobLauncher _jobLauncher;
        private readonly IConfigurationProvider _configuration;

        public TaskManager(
            TaskRepository taskRepository,
            TaskPool taskPool,
            JobLauncher jobLauncher,
            IConfigurationProvider configuration)
        {
            _taskRepository = taskRepository;
            _taskPool = taskPool;
            _jobLauncher = jobLauncher;
            _configuration = configuration;
        }

        public void GetTasks()
        {
            var existingTasks = _taskPool.GetTasks();

            var tasks =
                _taskRepository.GetTasksAsync(_configuration.AppSettings["WorkerId"], _taskPool.GetAvailableQuota()).Result;

            var newTasks = tasks.Where(t => existingTasks.All(tt => tt.Id != t.Id));
            
            var addedTasks = _taskPool.AddTasks(newTasks).ToList();

            addedTasks.ForEach(task => _jobLauncher.AddNewTaskJob(task));
        }

        public Task GetTask(int id)
        {
            return _taskPool.GetTask(id);
        }

        public void UpdateTask(Task task)
        {
            _taskRepository.UpdateTaskAsync(task).Wait();

            _taskPool.RemoveTask(task.Id);

            GetTasks();
        }
    }
}
