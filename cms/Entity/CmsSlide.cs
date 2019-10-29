using System;

namespace CMS.Entity
{
    public class CmsSlide
    {
        public Guid Id { get; set; }
        public int SlideOrder { get; set; }
        public bool HasLink { get; set; }
        public string LinkUrl { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public CmsSlideStatus Status { get; set; }
        public string Extension { get; set; }

        public string FileName { get { return string.Format("{0}.{1}", Id, Extension).ToLower(); } }
    }

    public enum CmsSlideStatus
    {
        Published = 1, Draft = 2
    }
}
