using System;
using System.Collections.Generic;

namespace BaseApp.Entity
{
    public class AppMessage
    {
        public Guid Id { get; set; }
        public Guid? ReferenceId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? CreatorId { get; set; }
        public string CreatorEmail { get; set; }
        public AppMessageStatus Status { get; set; }
        public bool IsRead { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public Guid? ContactId { get; set; }
        public string Email { get; set; }
        public string Recipients { get; set; }
        public string EmailRecipients { get; set; }
        public DateTime CreatedOn { get; set; }


        public string[] RecipientsList
        {
            get
            {
                if (string.IsNullOrEmpty(Recipients))
                {
                    return null;
                }
                else
                {
                    return Recipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        public string[] EmailRecipientsList
        {
            get
            {
                if (string.IsNullOrEmpty(EmailRecipients))
                {
                    return null;
                }
                else
                {
                    return EmailRecipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
        public virtual List<AppUser> RecipientContacts { get; set; }
        public virtual AppUser Creator { get; set; }
        public virtual AppUser Contact { get; set; }
        public virtual AppMessage Parent { get; set; }
        public virtual List<AppMessage> Children { get; set; }

        public virtual List<AppMessageAttachment> Attachments { get; set; }
    }


    public enum AppMessageStatus
    {
        Draft = 0,
        Outbox = 1,
        Inbox = 2
    }

    public class AppMessageAttachment
    {
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }
        public string Type { get; set; }

        public string FileName { get { return string.Format("{0}.{1}", Id, Extension); } }
    }
}
