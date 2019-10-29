using CMS.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CMS.Logic
{
    public interface ICmsPage
    {
        [Sql("CmsPages_GetAll")]
        List<CmsPage> GetAll();

        [Sql("CmsPages_MainMenu")]
        List<CmsPage> MainMenu();

        [Sql("CmsPages_GetByParentId")]
        List<CmsPage> GetByParentId(Guid ParentId);

        [Sql("CmsPages_GetById")]
        CmsPage GetById(Guid Id);

        [Sql("CmsPages_GetBySlug")]
        CmsPage GetBySlug(string Slug);

        [Sql("CmsPages_Create")]
        Guid Create(CmsPage model);

        [Sql("CmsPages_Update")]
        void Update(CmsPage model);

        [Sql("CmsPages_UpdateContent")]
        void UpdateContent(Guid Id, string Contents, Guid UpdatedByUserId);

        [Sql("CmsPages_UpdateMetadata")]
        void UpdateMetadata(CmsPage model);

        [Sql("CmsPages_Delete")]
        void Delete(Guid Id);

        [Sql("CmsPages_ChangeStatus")]
        void ChangeStatus(Guid Id, CmsPageStatus Status, Guid UpdatedByUserId);

        [Sql("CmsPages_UniqueSlug")]
        bool UniqueSlug(Guid Id, string Slug);


        #region Cms Page Section
        [Sql("CmsPageSections_GetAll")]
        List<CmsPageSection> GetAllSections();

        [Sql("CmsPageSections_GetByPageId")]
        List<CmsPageSection> GetSectionsByPageId(Guid PageId);

        [Sql("CmsPageSections_GetById")]
        CmsPageSection GetSectionById(Guid Id);

        [Sql("CmsPageSections_Create")]
        Guid CreateSection(CmsPageSection model);

        [Sql("CmsPageSections_ChangeStatus")]
        void ChangeSectionStatus(Guid Id);

        [Sql("CmsPageSections_Update")]
        void UpdateSection(CmsPageSection model);

        [Sql("CmsPageSections_Delete")]
        void DeleteSection(Guid Id);
        #endregion

    }
}
