using System;

namespace BaseApp.Entity
{
    public class AppNotification
    {
        public Guid Id { get; set; }
        public Guid? OfficeId { get; set; }
        public Guid ContactId { get; set; }
        public Guid? ReferenceId { get; set; }
        public AppNotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public string Title { get; set; }
        public string ActionUrl { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Office Office { get; set; }
        public virtual AppUser Contact { get; set; }
        public virtual CRM.Entity.Contact CrmContact { get; set; }
    }

    public enum AppNotificationType
    {
        Alert = 1,
        Warning = 2
    }

    public class NotificaitionSettings
    {
        public Guid Id { get; set; }
        public string NotificationTitle { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public ApplyFor ApplyFor { get; set; }
        public bool IsEmail { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public long CreatedById { get; set; }
        public string NotificationKey { get; set; }
        public string ModuleId { get; set; }
        //public List<AppUserRole> CustomRoles { get; set; }
    }
    public enum ApplyFor
    {
        All = 1, Custom = 2
    }
}
