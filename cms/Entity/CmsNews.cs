using System;

namespace CMS.Entity
{
    public class CmsNews
    {
        public Guid Id { get; set; }
        public CmsNewsStatus Status { get; set; }
        public DateTime DateTime { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public enum CmsNewsStatus
    {
        Published = 1, Draft = 2
    }
}
