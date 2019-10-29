using CMS.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CMS.Logic
{
    public interface ICmsContent
    {
        [Sql("CmsContents_GetAll")]
        List<CmsContent> GetAll();

        [Sql("CmsContents_GetById")]
        CmsContent GetById(Guid Id);

        [Sql("CmsContents_GetByName")]
        CmsContent GetByName(string Name);

        [Sql("CmsContents_UniqueContent")]
        bool UniqueContent(Guid Id, string Name);

        [Sql("CmsContents_ChangeStatus")]
        void ChangeStatus(Guid Id, Guid UpdatedByUserId);

        [Sql("CmsContents_Create")]
        Guid Create(CmsContent model);

        [Sql("CmsContents_Update")]
        void Update(CmsContent model);

        [Sql("CmsContents_Delete")]
        void Delete(Guid Id);
    }
}
