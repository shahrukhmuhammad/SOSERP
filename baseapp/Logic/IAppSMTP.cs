using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace BaseApp.Logic
{
    public interface IAppSMTP
    {
        [Sql("AppSMTP_GetAll")]
        List<AppSMTP> GetAll();

        [Sql("AppSMTP_GetById")]
        AppSMTP GetById(Guid Id);

        [Sql("AppSMTP_GetByTitle")]
        AppSMTP GetByTitle(string Title);

        [Sql("AppSMTP_UniqueTitle")]
        bool UniqueTitle(Guid Id, string Title);

        [Sql("AppSMTP_Create")]
        Guid Create(AppSMTP model);

        [Sql("AppSMTP_Update")]
        void Update(AppSMTP model);

        [Sql("AppSMTP_Delete")]
        void Delete(Guid Id);

        [Sql("AppSMTP_UpdateStatus")]
        void UpdateStatus(Guid Id);

        [Sql("AppSMTP_GetByThirdParty")]
        AppSMTP GetByThirdParty(int Id);

        [Sql("AppModuleSMTP_Create")]
        long Create(AppModuleSMTP model);

        [Sql("AppModuleSMTP_GetAll")]
        List<AppModuleSMTP> GetModules();
    }
}
