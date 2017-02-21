using System.Data.Entity;

namespace Tasker.DataContext.Configuration
{
    public class TaskContextConfiguration : DbConfiguration
    {
        public TaskContextConfiguration()
        {
            SetDatabaseInitializer(new DropCreateDatabaseIfModelChanges<TaskContext>());
        }
    }
}
