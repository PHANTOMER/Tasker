using Tasker.Business.Utils;
using Tasker.Common.Configuration;
using Tasker.Common.Extensions;
using Tasker.DataLayer;

namespace Tasker.Business.Tasks
{
    class SendEmailTaskExecutor : ITaskExecutor
    {
        private readonly EmailUtils _emailUtils;
        private readonly IConfigurationProvider _configuration;

        public SendEmailTaskExecutor(EmailUtils emailUtils, IConfigurationProvider configuration)
        {
            _emailUtils = emailUtils;
            _configuration = configuration;
        }

        public bool CanExecute(Task task)
        {
            return task.TaskType == TaskType.SendEmail;
        }

        public bool Execute(Task task)
        {
            TaskEmailData emailInfo = task.Parameter.DeserializeJson<TaskEmailData>();

            EmailData emailData = new EmailData()
            {
                Body = emailInfo.Body,
                Subject = emailInfo.Subject,
                ToAddress = emailInfo.Email,
                FromAddress = _configuration.AppSettings["EmailFromAddress"]
            };

            return _emailUtils.SendEmail(emailData);
        }
    }
}