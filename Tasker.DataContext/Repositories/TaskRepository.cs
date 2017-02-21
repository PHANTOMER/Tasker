using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Tasker.DataContext.UnitOfWork;
using Task = Tasker.DataLayer.Task;
using TaskStatus = Tasker.DataLayer.TaskStatus;

namespace Tasker.DataContext.Repositories
{
    public class TaskRepository
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TaskRepository(ILogger logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<DataLayer.Task> GetTaskAsync(int id)
        {
            using (var context = _unitOfWork.Create())
            {
                return await context.Set<Task>().FirstOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
            }    
        } 

        public async Task<bool> UpdateTaskAsync(Task task)
        {
            using (var context = _unitOfWork.Create())
            {
                try
                {
                    return await context.Attach(task).Update(task).CommitAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return false;
                }
            }
        } 

        public async Task<IEnumerable<DataLayer.Task>> GetTasksAsync(string workerId, int limit = 5)
        {
            using (var context = _unitOfWork.Create())
            {
                try
                {
                    var tasks =
                        await
                            context.Set<DataLayer.Task>()
                                .Where(
                                    x =>
                                        x.TaskStatus == TaskStatus.Created &&
                                        (x.WorkerId == null || x.WorkerId == workerId))
                                .OrderBy(x => x.StartsAfter)
                                .Take(limit)
                                .ToListAsync()
                                .ConfigureAwait(false);

                    var newTasks = tasks.Where(x => x.WorkerId == null).ToList();
                    
                    newTasks.ForEach(t => t.WorkerId = workerId);

                    if (newTasks.Count > 0)
                        await context.UpdateRange(newTasks).CommitAsync().ConfigureAwait(false);

                    return tasks;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.Error(ex);

                    foreach (var task in ex.Entries.Select(e => e.Entity).OfType<DataLayer.Task>())
                    {
                        _logger.Error($"Task {task.Id} was already taken");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }

            return Enumerable.Empty<Task>();
        } 
    }
}
