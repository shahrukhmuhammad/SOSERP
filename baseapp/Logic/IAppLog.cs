using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace BaseApp.Logic
{
    public interface IAppLog
    {
        [Sql("AppLogs_GetAll")]
        List<AppLog> GetAll();

        [Sql("AppLogs_GetByOfficeId")]
        List<AppLog> GetByOfficeId(Guid OfficeId);

        [Sql("AppLogs_GetByType")]
        List<AppLog> GetByType(AppLogType Type);

        [Sql("AppLogs_GetByTypeAndContactId")]
        List<AppLog> GetByTypeAndContactId(AppLogType Type, Guid ContactId);

        [Sql("AppLogs_GetByTypeAndOfficeId")]
        List<AppLog> GetByTypeAndOfficeId(AppLogType Type, Guid OfficeId);

        [Sql("AppLogs_GetById")]
        AppLog GetById(Guid Id);

        [Sql("AppLogs_GetByReferenceId")]
        List<AppLog> GetByReferenceId(Guid ReferenceId);

        [Sql("AppLogs_GetByContactId")]
        List<AppLog> GetByContactId(Guid ContactId);

        [Sql("AppLogs_Create")]
        Guid Create(
            Guid? OfficeId,
            Guid? ReferenceId,
            Guid? ContactId,
            AppLogType Type,
            string Module,
            string Title,
            string Location,
            string Description
        );

        [Sql("AppLogs_Create")]
        Guid Create(AppLog model);

        [Sql("AppLogs_Delete")]
        void Delete(Guid Id);

        [Sql("AppLogs_DeleteByContactId")]
        void DeleteByContactId(Guid ContactId);

        [Sql("AppLogs_DeleteByReferenceId")]
        void DeleteByReferenceId(Guid ReferenceId);
    }
}
