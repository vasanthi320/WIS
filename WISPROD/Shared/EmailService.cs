using NLog;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class EmailService
    {
        ILogger log;

        private SmtpClient mailer;

        public EmailService()
        {
            mailer = new SmtpClient();
            //this.log = log;
        }

        public void SendMail(MailMessage message)
        {
            try
            {
                log.Info(string.Format("Message Send Attempt:From={0};To={1};", message.From, message.To));
                mailer.Send(message);
            }
            catch (SmtpException ex)
            {

                //log.Error("Error in SendMail.");
                //log.Error(ex);
            }
        }
        public void SendMail(string address, string subject, string body, string replyTo = null, string cc = null, string fromName = null)
        {
            address = Utils.FixMailAddress(address, "hoar.com");
            var mailMessage = new MailMessage()
            {
                To = { new MailAddress(address) },
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            if (!String.IsNullOrEmpty(cc))
            {
                cc.Split(';').ToList().ForEach(addr =>
                {
                    addr = Utils.FixMailAddress(addr, "hoar.com");
                    mailMessage.CC.Add(new MailAddress(addr));
                });
            }

            if (!string.IsNullOrWhiteSpace(replyTo))
            {
                replyTo = Utils.FixMailAddress(replyTo, "hoar.com");
                mailMessage.ReplyToList.Add(new MailAddress(replyTo, fromName));
                mailMessage.From = new MailAddress(replyTo);
            }
            // log.Info(string.Format("Message Send Attempt:From={0};To={1};", mailMessage.From.ToString(), mailMessage.To.ToString()));

            try
            {
                mailer.Send(mailMessage);
            }
            catch (SmtpException ex)
            {

            }
        }
    }

}
