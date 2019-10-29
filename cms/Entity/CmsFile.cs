using System;

namespace CMS.Entity
{
    public class CmsFile
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public int Size { get; set; }
        public DateTime CreatedOn { get; set; }

        public string FileName { get { return string.Format("{0}.{1}", Id, Extension).ToLower(); } }
    }
}
