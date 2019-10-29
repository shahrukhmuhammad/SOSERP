using System;

namespace CMS.Entity
{
    public class CmsSeoMetadata
    {
        public Guid Id { get; set; }
        public Guid? PageId { get; set; }
        public string Title { get; set; }
        public string Metadata { get; set; }
        public bool AllPages { get; set; }

        public virtual CmsPage Page { get; set; }
    }
}
