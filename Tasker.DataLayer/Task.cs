using System;

namespace Tasker.DataLayer
{
    public enum TaskType
    {
        CreateFile = 1,
        SendEmail = 2
    }

    public enum TaskStatus
    {
        Created = 1,
        Completed = 2,
        Failed = 3,
    }

    public class TaskEmailData
    {
        public string Subject { get; set; }

        public string Email { get; set; }

        public string Body { get; set; }
    }

    public class Task
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Parameter { get; set; }

        public string WorkerId { get; set; }

        public DateTime StartsAfter { get; set; }

        public TaskType TaskType { get; set; }

        public TaskStatus TaskStatus { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
