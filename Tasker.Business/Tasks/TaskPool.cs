using System.Collections.Generic;
using System.Linq;
using Task = Tasker.DataLayer.Task;

namespace Tasker.Business.Tasks
{
    public class TaskPool
    {
        private readonly int _limit;
        private readonly object _lock = new object();

        private readonly List<DataLayer.Task> _tasks;

        public TaskPool(int limit)
        {
            _limit = limit;
            _tasks = new List<Task>();
        }

        public IEnumerable<Task> AddTasks(IEnumerable<Task> tasks)
        {
            lock (_lock)
            {
                if (_tasks.Count >= _limit)
                    yield break;

                foreach (var task in tasks.Where(task => !_tasks.Exists(t => t.Id == task.Id)).Take(_limit - _tasks.Count))
                {
                    _tasks.Add(task);
                    yield return task;
                }
            }
        }

        public void RemoveTask(int id)
        {
            lock (_lock)
            {
                var task = GetTask(id);
                if (task != null)
                    _tasks.Remove(task);
            }
        }

        public int GetCount()
        {
            lock (_lock)
            {
                return _tasks.Count;
            }
        }

        public int GetAvailableQuota()
        {
            return _limit - GetCount();
        }

        public Task GetTask(int id)
        {
            Task task = null;
            lock (_lock)
            {
                task = _tasks.FirstOrDefault(t => t.Id == id);
            }

            return task;
        }

        public IEnumerable<Task> GetTasks()
        {
            lock (_lock)
            {
                return _tasks.ToList();
            }
        } 
    }
}
