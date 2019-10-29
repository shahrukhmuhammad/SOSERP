using CMS.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CMS.Logic
{
    public interface ICmsNews
    {
        [Sql("CmsNews_GetAll")]
        List<CmsNews> GetAll();

        [Sql("CmsNews_GetById")]
        CmsNews GetById(Guid Id);

        [Sql("CmsNews_Create")]
        Guid Create(CmsNews model);

        [Sql("CmsNews_ChangeStatus")]
        void ChangeStatus(Guid Id, Guid UpdatedByUserId);

        [Sql("CmsNews_Update")]
        void Update(CmsNews model);

        [Sql("CmsNews_Delete")]
        void Delete(Guid Id);
    }
}
