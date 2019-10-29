using CMS.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CMS.Logic
{
    public interface ICmsSeoAccount
    {
        [Sql("CmsSeoAccounts_GetAll")]
        List<CmsSeoAccount> GetAll();

        [Sql("CmsSeoAccounts_GetByProvider")]
        CmsSeoAccount GetByProvider(string Provider);

        [Sql("CmsSeoAccounts_GetById")]
        CmsSeoAccount GetById(Guid Id);

        [Sql("CmsSeoAccounts_Update")]
        void Update(CmsSeoAccount model);




        [Sql("CmsSeoAccountDetails_GetByProviderId")]
        List<CmsSeoAccountDetail> GetServicesByProviderId(Guid ProviderId);

        [Sql("CmsSeoAccountDetails_GetById")]
        CmsSeoAccountDetail GetServiceById(Guid Id);

        [Sql("CmsSeoAccountDetails_Update")]
        void UpdateService(CmsSeoAccountDetail model);
    }
}
