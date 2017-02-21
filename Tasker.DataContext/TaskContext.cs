using System.Data.Entity;
using Tasker.DataContext.Configuration;

namespace Tasker.DataContext
{
    [DbConfigurationType(typeof(TaskContextConfiguration))]
    public class TaskContext : DbContext
    {
        public TaskContext() : base("DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public TaskContext(string cstring) : base(cstring)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataLayer.Task>().Property(p => p.RowVersion).IsRowVersion();

            base.OnModelCreating(modelBuilder);
        }
    }
}
