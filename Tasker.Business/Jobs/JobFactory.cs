using System;
using Quartz;

namespace Tasker.Business.Jobs
{
    public class TaskJobData
    {
        public int TaskId { get; set; }

        public DateTime StartAt { get; set; }
    }
    
    public class JobFactory
    {
        public void CreateTaskExecutionJob(IScheduler scheduler, TaskJobData jobData)
        {
            var job = JobBuilder.Create<TaskExecutionJob>()
                .RequestRecovery()
                .UsingJobData("Id", jobData.TaskId)
                .Build();

            // Associate a trigger with the Job

            ITrigger trigger;

            if (DateTime.UtcNow > jobData.StartAt)
                trigger = TriggerBuilder.Create()
                    .StartNow()
                    .Build();
            else
            {
                trigger = TriggerBuilder.Create()
                   .StartAt(jobData.StartAt)
                   .Build();
            }

            scheduler.ScheduleJob(job, trigger);
        }
    }
}
