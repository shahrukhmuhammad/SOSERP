using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace BaseApp.Logic
{
    public interface IAppMessage
    {
        [Sql("AppMessages_GetByContactIdAndStatus")]
        List<AppMessage> GetByContactIdAndStatus(Guid ContactId, AppMessageStatus Status);

        [Sql("AppMessages_GetByParentId")]
        List<AppMessage> GetByParentId(Guid ParentId);

        [Sql("AppMessages_GetById")]
        AppMessage GetById(Guid Id);

        [Sql("AppMessages_Create")]
        Guid Create(AppMessage model);

        [Sql("AppMessages_Create")]
        Guid Create(
            Guid? ReferenceId,
            Guid? ParentId,
            Guid? CreatorId,
            string CreatorEmail,
            AppMessageStatus Status,
            string Subject,
            string Message,
            Guid? ContactId,
            string Email,
            string Recipients,
            string EmailRecipients
            );

        [Sql("AppMessages_Create")]
        Guid Create(
            Guid? ReferenceId,
            Guid? ParentId,
            Guid? CreatorId,
            string CreatorEmail,
            AppMessageStatus Status,
            string Subject,
            Guid? ContactId,
            string Email
            );

        [Sql("AppMessages_Update")]
        void Update(AppMessage model);

        [Sql("AppMessages_MarkAllRead")]
        void MarkAllRead(Guid ContactId);

        [Sql("AppMessages_MarkRead")]
        void MarkRead(Guid Id);

        [Sql("AppMessages_DeleteByParentId")]
        void DeleteByParentId(Guid ParentId);

        [Sql("AppMessages_DeleteByContactId")]
        void DeleteByContactId(Guid ContactId);

        [Sql("AppMessages_Delete")]
        void Delete(Guid Id);



        #region Attachments
        [Sql("AppMessageAttachments_GetById")]
        AppMessageAttachment GetAttachmentById(Guid Id);

        [Sql("AppMessageAttachments_GetByMessageId")]
        List<AppMessageAttachment> GetAttachmentsByMessageId(Guid MessageId);

        [Sql("AppMessageAttachments_Create")]
        Guid CreateAttachment(AppMessageAttachment model);

        [Sql("AppMessageAttachments_Create")]
        Guid CreateAttachment(Guid MessageId, string Title, string Type, string Extension);

        [Sql("AppMessageAttachments_DeleteByMessageId")]
        void DeleteAttachmentsByMessageId(Guid MessageId);

        [Sql("AppMessageAttachments_Delete")]
        void DeleteAttachment(Guid Id);
        #endregion
    }
}
