using CMS.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CMS.Logic
{
    public interface ICmsNewsletter
    {
        #region Newsletter
        [Sql("CmsNewsletter_GetAll")]
        List<CmsNewsletter> GetAll();

        [Sql("CmsNewsletter_GetById")]
        CmsNewsletter GetById(Guid Id);

        [Sql("CmsNewsletter_Create")]
        Guid Create(CmsNewsletter model);

        [Sql("CmsNewsletter_Update")]
        void Update(CmsNewsletter model);

        [Sql("CmsNewsletter_Delete")]
        void Delete(Guid Id);
        #endregion


        #region Newsletter Email
        [Sql("CmsNewsletterEmails_GetAll")]
        List<CmsNewsletterEmail> GetAllEmails();

        [Sql("CmsNewsletterEmails_GetById")]
        CmsNewsletterEmail GetEmailById(Guid Id);

        [Sql("CmsNewsletterEmails_GetByEmail")]
        CmsNewsletterEmail GetEmailByEmail(string Email);

        [Sql("CmsNewsletterEmails_Create")]
        Guid CreateEmail(string Email);

        [Sql("CmsNewsletterEmails_CheckDuplicateEmail")]
        bool CheckDuplicateEmail(Guid Id, string Email);

        [Sql("CmsNewsletterEmails_DeleteById")]
        void DeleteEmailById(Guid Id);

        [Sql("CmsNewsletterEmails_DeleteByEmail")]
        void DeleteEmailByEmail(string Email);
        #endregion
    }
}
