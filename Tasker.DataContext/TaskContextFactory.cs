using System.Data.Entity.Infrastructure;

namespace Tasker.DataContext
{
    public class TaskContextFactory : IDbContextFactory<TaskContext>
    {
        private readonly string _cstring;

        public TaskContextFactory(string cstring)
        {
            _cstring = cstring;
        }

        public TaskContext Create()
        {
            return new TaskContext(_cstring);
        }
    }
}
