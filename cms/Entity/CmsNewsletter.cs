using System;

namespace CMS.Entity
{
    public class CmsNewsletter
    {
        public Guid Id { get; set; }
        public CmsNewsletterStatus Status { get; set; }
        public string Subject { get; set; }
        public string Contents { get; set; }
        public string Recipients { get; set; }
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
        public DateTime CreatedOn { get; set; }
        public Guid CreatedByUserId { get; set; }
    }

    public enum CmsNewsletterStatus
    {
        Draft = 1,
        Sent = 2
    }

    public class CmsNewsletterEmail
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
