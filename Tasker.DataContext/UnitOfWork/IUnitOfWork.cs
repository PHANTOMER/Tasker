using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace Tasker.DataContext.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUnitOfWork Create();

        Task<bool> CommitAsync();

        IQueryable<TItem> Set<TItem>() where TItem : class;

        IUnitOfWork Attach<TItem>(TItem item) where TItem : class;

        IUnitOfWork AttachRange<TItem>(IEnumerable<TItem> items) where TItem : class;

        IUnitOfWork Update<TItem>(TItem item) where TItem : class;

        IUnitOfWork UpdateRange<TItem>(IEnumerable<TItem> items) where TItem : class;
    }

    class EntityUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly IDbContextFactory<TContext> _contextFactory;
        private TContext _context;

        public EntityUnitOfWork(IDbContextFactory<TContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
                _context = null;
            }
        }

        public IUnitOfWork Create()
        {
            if (_context != null)
                throw new Exception("Context was created already");

            _context = _contextFactory.Create();
            return this;
        }

        public IQueryable<TItem> Set<TItem>() where TItem : class
        {
            return _context.Set<TItem>();
        }

        public IUnitOfWork Attach<TItem>(TItem item) where TItem : class
        {
            _context.Set<TItem>().Attach(item);
            return this;
        }

        public IUnitOfWork AttachRange<TItem>(IEnumerable<TItem> items) where TItem : class
        {
            var set = _context.Set<TItem>();
            foreach (var item in items)
            {
                set.Attach(item);
            }

            return this;
        }

        public IUnitOfWork Update<TItem>(TItem item) where TItem : class
        {
            _context.Entry(item).State = EntityState.Modified;
            return this;
        }

        public IUnitOfWork UpdateRange<TItem>(IEnumerable<TItem> items) where TItem : class
        {
            foreach (var item in items)
            {
                _context.Entry(item).State = EntityState.Modified;
            }

            return this;
        }

        public async Task<bool> CommitAsync()
        {
            _context.ChangeTracker.DetectChanges();
            return await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }
    }
}
