using Tasker.DataLayer;

namespace Tasker.Business.Tasks
{
    public interface ITaskExecutor
    {
        bool CanExecute(DataLayer.Task task);

        bool Execute(DataLayer.Task task);
    }
}
