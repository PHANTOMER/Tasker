using System;
using System.Net;
using System.Net.Mail;
using Tasker.Common.Configuration;
using Tasker.Common.Extensions;
using ILogger = Autofac.Extras.NLog.ILogger;

namespace Tasker.Business.Utils
{
    public class EmailData
    {
        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public string FromDisplayName { get; set; }

        public string ToDisplayName { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }

    public class EmailUtils
    {
        private readonly IConfigurationProvider _configuration;
        private readonly ILogger _logger;

        public EmailUtils(IConfigurationProvider configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }


        public bool SendEmail(EmailData emailData)
        {
            bool isOk = false;

            var fromAddressObj = new MailAddress(emailData.FromAddress, emailData.FromDisplayName ?? emailData.FromAddress);
            var toAddressObj = new MailAddress(emailData.ToAddress, emailData.ToDisplayName ?? emailData.ToAddress);

            int MaxTryCount = 3;
            int tryCount = 0;
            while (!isOk && tryCount < MaxTryCount)
            {
                try
                {
                    var smtp = new SmtpClient
                    {
                        Host = _configuration.AppSettings["SmtpHost"],
                        Port = _configuration.AppSettings["SmtpPort"].ToInt(),
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(_configuration.AppSettings["SmtpUsername"], _configuration.AppSettings["SmtpPassword"])
                    };

                    using (var message = new MailMessage(fromAddressObj, toAddressObj)
                    {
                        Subject = emailData.Subject,
                        Body = emailData.Body,
                    })
                    {
                        long size = 0;

                        smtp.Send(message);
                        isOk = true;
                    }

                    _logger.Info("Email was sent successfully");
                }
                catch (Exception exc)
                {
                    if (tryCount < MaxTryCount)
                        tryCount++;
                    else
                    {
                        _logger.Error("Error sending email report after {0} retries. Error: {1}",
                            MaxTryCount,
                            exc.Message);
                    }
                }
            }

            return isOk;
        }
    }
}
