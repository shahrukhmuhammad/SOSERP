using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace BaseApp.Logic
{
    public interface IExtraFieldSection
    {
        [Sql("ExtraFieldSections_GetByModule")]
        List<ExtraFieldSection> GetByModule(string Module);

        [Sql("ExtraFieldSections_GetByParentId")]
        List<ExtraFieldSection> GetByParentId(Guid ParentId);

        [Sql("ExtraFieldSections_GetById")]
        ExtraFieldSection GetById(Guid Id);

        [Sql("ExtraFieldSections_Create")]
        Guid Create(ExtraFieldSection model);

        [Sql("ExtraFieldSections_Update")]
        void Update(ExtraFieldSection model);

        [Sql("ExtraFieldSections_Delete")]
        void Delete(Guid Id);

        [Sql("ExtraFieldSections_DeleteByParentId")]
        void DeleteByParentId(Guid ParentId);
    }
}
