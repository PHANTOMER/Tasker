using System;
using System.IO;
using Autofac.Extras.NLog;
using Tasker.DataLayer;
using Task = System.Threading.Tasks.Task;

namespace Tasker.Business.Tasks
{
    class CreateFileTaskExecutor : ITaskExecutor
    {
        private readonly ILogger _logger;

        public CreateFileTaskExecutor(ILogger logger)
        {
            _logger = logger;
        }

        public bool CanExecute(DataLayer.Task task)
        {
            return task.TaskType == TaskType.CreateFile;
        }

        public bool Execute(DataLayer.Task task)
        {
            Task.Delay(TimeSpan.FromSeconds(10)).Wait();
            try
            {
                string folderPath = Path.Combine(Path.GetTempPath(), "TaskerData");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                File.Create(Path.Combine(folderPath, task.Parameter)).Close();
                _logger.Info($"File {task.Parameter} created successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return false;
            }

            return true;
        }
    }
}