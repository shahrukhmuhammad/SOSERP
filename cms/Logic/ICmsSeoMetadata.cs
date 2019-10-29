using CMS.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CMS.Logic
{
    public interface ICmsSeoMetadata
    {
        [Sql("CmsSeoMetadata_WebsiteMetadata")]
        List<CmsSeoMetadata> WebsiteMetadata();

        [Sql("CmsSeoMetadata_PageMetadata")]
        List<CmsSeoMetadata> PageMetadata(Guid PageId);

        [Sql("CmsSeoMetadata_Create")]
        Guid Create(CmsSeoMetadata model);

        [Sql("CmsSeoMetadata_DeleteWebsite")]
        void DeleteWebsite();

        [Sql("CmsSeoMetadata_DeletePage")]
        void DeletePage(Guid PageId);
    }
}
