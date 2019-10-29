using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace BaseApp
{
    public interface IEmailService
    {
        void Send(string toEmail, string body, string subject = null);
        void Send(string toEmail, string body, HttpPostedFileBase[] attachments, string subject = null);
        void Send(string toEmail, string template, object model, string subject = null);
        void Send(string toEmail, string template, object model, IEnumerable<Attachment> attachments);
        void Send(SmtpClient userSmtpClient, MailMessage userMailMessage, string toEmail, string ccTo, string subject, string body, IEnumerable<Attachment> attachments);
    }
}

namespace BaseApp.System
{
    public class SmtpEmailService : IEmailService
    {
        private SmtpClient smtpClient;
        private MailMessage mailMessage;
        private IAppSettings appSettings;
        private IEmailTemplateRepository repo;

        public SmtpEmailService(IDbConnection db, IAppSettings appSettings)
        {
            this.appSettings = appSettings;
            repo = db.As<IEmailTemplateRepository>();

            var emailServer = appSettings.GetVal("Smtp:Host");
            var emailPort = appSettings.GetVal<int>("Smtp:Port");
            var emailSsl = appSettings.GetVal<bool>("Smtp:Ssl");
            var emailUser = appSettings.GetVal("Smtp:Username");
            var emailPassword = appSettings.GetVal("Smtp:Password");

            smtpClient = new SmtpClient
            {
                Host = emailServer,
                Port = emailPort,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailUser, emailPassword),
                EnableSsl = emailSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var emailFrom = appSettings.GetVal("Email:From");
            var emailFrmName = appSettings.GetVal("Email:FromName");

            mailMessage = new MailMessage
            {
                From = new MailAddress(emailFrom, emailFrmName),
                IsBodyHtml = true
            };
        }

        public void Send(string toEmail, string body, string subject = null)
        {
            mailMessage.To.Clear();
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {

            }
        }

        public void Send(string toEmail, string body, HttpPostedFileBase[] attachments, string subject = null)
        {
            mailMessage.To.Clear();
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = body;
            foreach (var x in attachments)
            {
                if (x != null && x.ContentLength > 0)
                {
                    var attch = new Attachment(x.InputStream, x.FileName);
                    mailMessage.Attachments.Add(attch);
                }
            }

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {

            }
        }

        public void Send(string toEmail, string template, object modal, string subject = null)
        {
            string body;
            var templateId = template.Contains(" ") ? template.Replace(" ", null) : template;
            var emailTemplate = repo.GetById(templateId);
            if (emailTemplate != null)
            {
                body = RazorEngine.Razor.Parse(HttpUtility.HtmlDecode(emailTemplate.BodyContent), modal);
                subject = subject ?? emailTemplate.Subject;
            }
            else
            {
                body = RazorEngine.Razor.Parse(template, modal);
            }
            mailMessage.To.Clear();
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {

            }
        }

        public void Send(string toEmail, string template, object modal, IEnumerable<Attachment> attachments)
        {
            string body;
            string subject = string.Empty;

            var templateId = template.Contains(" ") ? template.Replace(" ", null) : template;
            var emailTemplate = repo.GetById(templateId);
            if (emailTemplate != null)
            {
                body = RazorEngine.Razor.Parse(HttpUtility.HtmlDecode(emailTemplate.BodyContent), modal); //RazorEngine.Engine.Razor.RunCompile(System.Web.HttpUtility.HtmlDecode(emailTemplate.BodyContent), templateId, null, modal);
                subject = emailTemplate.Subject;
            }
            else
            {
                body = RazorEngine.Razor.Parse(template, modal);
            }
            mailMessage.To.Clear();
            if (toEmail.Contains(","))
            {
                foreach (var x in toEmail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mailMessage.To.Add(x);
                }
            }
            else
            {
                mailMessage.To.Add(toEmail);
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            foreach (var x in attachments)
                mailMessage.Attachments.Add(x);
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {

            }
        }

        public void Send(SmtpClient userSmtpClient, MailMessage userMailMessage, string toEmail, string ccTo, string subject, string body, IEnumerable<Attachment> attachments)
        {
            userMailMessage.To.Clear();
            if (toEmail.Contains(","))
            {
                foreach (var x in toEmail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    userMailMessage.To.Add(x);
                }
            }
            else
            {
                userMailMessage.To.Add(toEmail);
            }
            if (ccTo.HasValue())
            {
                userMailMessage.CC.Clear();
                userMailMessage.CC.Add(ccTo);
            }
            userMailMessage.Subject = subject;
            userMailMessage.Body = body;
            foreach (var x in attachments)
                userMailMessage.Attachments.Add(x);

            try
            {
                userSmtpClient.Send(userMailMessage);
            }
            catch (Exception ex)
            {

            }
        }
    }

    public interface IEmailTemplateRepository
    {
        [Sql("EmailTemplates_GetById")]
        EmailTemplate GetById(string id);

        [Sql("EmailTemplates_Update")]
        void Save(EmailTemplate emailTemplate);
    }
}
