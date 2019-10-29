using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace BaseApp.Logic
{
    public interface IAppNotification
    {
        [Sql("AppNotifications_Create")]
        Guid Create(Guid? OfficeId, Guid ContactId, Guid? ReferenceId, AppNotificationType Type, string Title, string ActionUrl, string Description);

        [Sql("AppNotifications_Create")]
        Guid Create(AppNotification model);

        [Sql("AppNotifications_GetAll")]
        List<AppNotification> GetAll();

        [Sql("AppNotifications_GetByContactId")]
        List<AppNotification> GetByContactId(Guid ContactId);

        [Sql("AppNotifications_GetRecentByContactIdAndType")]
        List<AppNotification> GetRecentByContactIdAndType(Guid ContactId, AppNotificationType Type);

        [Sql("AppNotifications_GetByContactIdAndType")]
        List<AppNotification> GetByContactIdAndType(Guid ContactId, AppNotificationType Type);

        [Sql("AppNotifications_GetById")]
        AppNotification GetById(Guid Id);

        [Sql("AppNotifications_GetByOfficeId")]
        List<AppNotification> GetByOfficeId(Guid OfficeId);

        [Sql("AppNotifications_MarkRead")]
        void MarkRead(Guid Id);

        [Sql("AppNotifications_ReadAllByContactId")]
        void ReadAllByContactId(Guid ContactId);

        [Sql("AppNotifications_Delete")]
        void Delete(Guid Id);

        [Sql("AppSettingNotifications_GetAllByModule")]
        List<NotificaitionSettings> GetAllByModule(string Id);

        [Sql("AppSettingNotifications_GetById")]
        NotificaitionSettings GetNotifyById(Guid Id);
    }
}
