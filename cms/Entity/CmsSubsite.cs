using System;
using System.Collections.Generic;

namespace CMS.Entity
{
    public class CmsSubsite
    {
        public Guid Id { get; set; }
        public bool IsSystem { get; set; }
        public CmsSubsiteStatus Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual CmsPage LandingPage { get; set; }
        public virtual List<CmsPage> Pages { get; set; }
    }
    public enum CmsSubsiteStatus
    {
        Published = 1,
        UnPublished = 2
    }

}
