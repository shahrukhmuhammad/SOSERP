using BaseApp.Entity;
using System;
using System.Collections.Generic;

namespace CMS.Entity
{
    public class CmsPage
    {
        public Guid Id { get; set; }
        public Guid? SubsiteId { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }
        public bool IsSystem { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public bool IsLink { get; set; }
        public string LinkUrl { get; set; }
        public bool LinkTarget { get; set; }
        public int PageOrder { get; set; }
        public CmsPageStatus Status { get; set; }
        public string BrowserTitle { get; set; }
        public string Metadata { get; set; }
        public string Contents { get; set; }

        public string Tags { get; set; }
        public string[] TagsList
        {
            get
            {
                if (string.IsNullOrEmpty(Tags))
                {
                    return null;
                }
                else
                {
                    return Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }

        public bool Menu { get; set; }
        public bool Footer { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public Guid CreatedByUserId { get; set; }

        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }


        public virtual AppUser UpdatedByUser { get; set; }
        public virtual AppUser CreatedByUser { get; set; }


        public virtual CmsSubsite Subsite { get; set; }
        public virtual CmsPage Parent { get; set; }
        public virtual List<CmsPage> ChildPages { get; set; }
        public virtual List<CmsPageSection> PageSections { get; set; }
        public virtual List<CmsSeoMetadata> SeoMetadata { get; set; }
    }
    public enum CmsPageStatus
    {
        Published = 1, Draft = 2
    }

    #region Cms Page Sections
    public class CmsPageSection
    {
        public Guid Id { get; set; }
        public Guid PageId { get; set; }
        public CmsPageSectionStatus Status { get; set; }
        public int SectionOrder { get; set; }
        public string Name { get; set; }
        public string Section { get; set; }

        public virtual CmsPage Page { get; set; }
    }

    public enum CmsPageSectionStatus
    {
        Published = 1, Draft = 2
    }
    #endregion
}
