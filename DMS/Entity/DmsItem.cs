using System;

namespace DMS.Entity
{
    public class DmsItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid? ParentId { get; set; }
        public string Extension { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DmsResourceType ResourceType { get; set; }
        public string ContentType { get; set; }
        public string Type
        {
            get
            {
                return ResourceType == DmsResourceType.Folder ? "File folder" : Extension;
            }
        }
        public string Filename
        {
            get
            {
                return ResourceType == DmsResourceType.Folder ? Title : string.Format("{0}.{1}", Title, Extension);
            }
        }

        public Int64 FileSize { get; set; }
        public Guid? CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public int Version { get; set; }
        public string Path { get; set; }
        public bool IsSystem { get; set; }
    }

    public enum DmsResourceType
    {
        File, Folder
    }

    public enum DmsRenderMode
    {
        View, Browse
    }
}
