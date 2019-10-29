using CMS.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CMS.Logic
{
    public interface ICmsSlide
    {
        [Sql("CmsSlides_GetById")]
        CmsSlide GetById(Guid Id);

        [Sql("CmsSlides_GetAll")]
        List<CmsSlide> GetAll();

        [Sql("CmsSlides_ChangeStatus")]
        void ChangeStatus(Guid Id);

        [Sql("CmsSlides_Create")]
        Guid Create(CmsSlide model);

        [Sql("CmsSlides_Update")]
        void Update(CmsSlide model);

        [Sql("CmsSlides_Delete")]
        void Delete(Guid Id);
    }
}
