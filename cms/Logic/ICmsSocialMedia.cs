using CMS.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CMS.Logic
{
    public interface ICmsSocialMedia
    {
        [Sql("CmsSocialMedia_GetAll")]
        List<CmsSocialMedia> GetAll();

        [Sql("CmsSocialMedia_GetById")]
        CmsSocialMedia GetById(Guid Id);

        [Sql("CmsSocialMedia_Create")]
        void Create(CmsSocialMedia model);

        [Sql("CmsSocialMedia_Update")]
        void Update(CmsSocialMedia model);

        [Sql("CmsSocialMedia_Delete")]
        void Delete(Guid Id);
    }
}
