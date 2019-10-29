using System;

namespace CMS.Entity
{
    public class CmsContent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public CmsContentStatus Status { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public enum CmsContentStatus
    {
        Published = 1, Draft = 2
    }
}
