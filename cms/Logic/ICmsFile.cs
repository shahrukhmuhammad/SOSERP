using CMS.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CMS.Logic
{
    public interface ICmsFile
    {
        [Sql("CmsFiles_GetAll")]
        List<CmsFile> GetAll();

        [Sql("CmsFiles_GetById")]
        CmsFile GetById(Guid Id);

        [Sql("CmsFiles_Create")]
        Guid Create(CmsFile model);

        [Sql("CmsFiles_Delete")]
        void Delete(Guid Id);
    }
}
