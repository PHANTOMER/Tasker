using System;
using Autofac.Extras.NLog;
using Quartz;
using Quartz.Impl;
using Tasker.Business.DI;
using Tasker.Common.Autofac;

namespace Tasker.Business.Jobs
{
    public interface ISchedulerFactory
    {
        IScheduler Create();
    }

    class SchedulerFactory : ISchedulerFactory
    {
        private readonly ILogger _logger;

        public SchedulerFactory(ILogger logger)
        {
            _logger = logger;
        }

        public IScheduler Create()
        {
            try
            {
                // Get a reference to the scheduler
                var sf = new StdSchedulerFactory();
                var scheduler = sf.GetScheduler();

                var scope = AutofacCore.Core.BeginLifetimeScope();

                scheduler.JobFactory = new AutoFacJobFactory(scope);
                return scheduler;
            }
            catch (Exception ex)
            {
                _logger.Fatal("Scheduler not available: '{0}'", ex.Message);
                throw;
            }
        }
    }
}
