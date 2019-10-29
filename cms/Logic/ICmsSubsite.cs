using CMS.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CMS.Logic
{
    public interface ICmsSubsite
    {
        [Sql("CmsSubsites_GetAll")]
        List<CmsSubsite> GetAll();

        [Sql("CmsSubsites_GetById")]
        CmsSubsite GetById(Guid Id);

        [Sql("CmsSubsites_GetByTitle")]
        CmsSubsite GetByTitle(string Title);

        [Sql("CmsSubsites_Create")]
        Guid Create(CmsSubsite model);

        [Sql("CmsSubsites_UniqueTitle")]
        bool UniqueTitle(Guid Id, string Title);

        [Sql("CmsSubsites_Update")]
        void Update(CmsSubsite model);

        [Sql("CmsSubsites_Delete")]
        void Delete(Guid Id);
    }
}
