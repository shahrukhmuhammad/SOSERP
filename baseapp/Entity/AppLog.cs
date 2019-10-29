using System;

namespace BaseApp.Entity
{
    public class AppLog
    {
        public Guid Id { get; set; }
        public Guid? OfficeId { get; set; }
        public Guid? ReferenceId { get; set; }
        public Guid? ContactId { get; set; }
        public AppLogType Type { get; set; }
        public string Module { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual AppUser Contact { get; set; }
        public virtual Office Office { get; set; }
    }

    public enum AppLogType
    {
        Activity = 1,
        Error = 2
    }

}
