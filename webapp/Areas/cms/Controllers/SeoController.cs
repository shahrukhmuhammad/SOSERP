using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using CMS.Logic;
using CMS.Entity;

namespace WebApp.Areas.CMS.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All, AppPermission.ViewCMS, AppPermission.CMS)]
    public class SeoController : AppController
    {
        private IAppLog appLog;
        private ICmsPage cmsPage;
        private ICmsSeoMetadata seoMetadata;
        private ICmsSeoAccount seoAccount;

        public SeoController()
        {
            appLog = db.As<IAppLog>();
            cmsPage = db.As<ICmsPage>();
            seoMetadata = db.As<ICmsSeoMetadata>();
            seoAccount = db.As<ICmsSeoAccount>();
        }

        #region Overview
        public ActionResult Index()
        {
            ViewBag.WebsiteMetadata = seoMetadata.WebsiteMetadata();
            var pagesList = cmsPage.GetAll();
            foreach (var x in pagesList)
            {
                x.SeoMetadata = seoMetadata.PageMetadata(x.Id);
            }
            ViewBag.CmsPages = pagesList;
            ViewBag.SeoAccounts = seoAccount.GetAll();

            return View();
        }
        #endregion

        #region Website Metadata

        #region Metadata
        public ActionResult Metadata()
        {
            var model = seoMetadata.WebsiteMetadata();
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Metadata(List<CmsSeoMetadata> model)
        {
            try
            {
                if (model.Count() > 0)
                {
                    seoMetadata.DeleteWebsite();
                    foreach (var x in model)
                    {
                        if (!string.IsNullOrEmpty(x.Title) && !string.IsNullOrEmpty(x.Metadata))
                        {
                            x.PageId = null;
                            x.Id = seoMetadata.Create(x);
                        }
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "CMS - Seo Metadata", "Website Seo / Metadata Updated", "~/CMS/Seo/Metadata > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Website SEO / metadata updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = "Website SEO / metadata has been updated successfully.";
                }
                else
                {
                    TempData["ErrorMsg"] = "There was no record found.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "CMS - Seo Metadata", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/Seo/Metadata > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Index");
        }
        #endregion


        #region Page Optimization
        public ActionResult UrlMetadata()
        {
            var model = cmsPage.GetAll();
            return View(model);
        }
        public ActionResult PageMetadata(Guid Id)
        {
            var model = cmsPage.GetById(Id);
            model.SeoMetadata = seoMetadata.PageMetadata(Id);
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult PageMetadata(CmsPage model)
        {
            try
            {
                model.UpdatedByUserId = CurrentUser.Id;
                cmsPage.UpdateMetadata(model);

                if (model.SeoMetadata.Count() > 0)
                {
                    seoMetadata.DeletePage(model.Id);
                    foreach (var x in model.SeoMetadata)
                    {
                        if (!string.IsNullOrEmpty(x.Title) && !string.IsNullOrEmpty(x.Metadata))
                        {
                            x.PageId = model.Id;
                            x.AllPages = false;
                            x.Id = seoMetadata.Create(x);
                        }
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "CMS - Page Metadata", model.Name + " Updated", "~/CMS/Seo/PageMetadata > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = model.Name + " updated successfully.";
                }
                else
                {
                    TempData["ErrorMsg"] = "There was no record found.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "CMS - Page Metadata", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/Seo/PageMetadata > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("UrlMetadata");
        }
        #endregion



        public ActionResult Keyword()
        {
            return View();
        }
        public ActionResult Sitemap()
        {
            return View();
        }
        #endregion

        #region Management
        public ActionResult Management()
        {
            var model = seoAccount.GetAll();
            foreach (var x in model)
            {
                x.Services = seoAccount.GetServicesByProviderId(x.Id);
            }
            return View(model);
        }

        public PartialViewResult _SeoAccount(Guid Id)
        {
            return PartialView(seoAccount.GetById(Id));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateSeoAccount(CmsSeoAccount model)
        {
            try
            {
                model.Status = CmsSeoAccountStatus.Configured;
                seoAccount.Update(model);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "CMS - Seo Account", model.Provider + " Updated", "~/CMS/Seo/UpdateSeoAccount > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Provider + "</strong> updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = model.Provider + " updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "CMS - Seo Account", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/Seo/UpdateSeoAccount > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Management");
        }

        #region Google
        [HttpPost]
        public JsonResult DisableGoogleService(Guid Id)
        {
            var model = seoAccount.GetServiceById(Id);
            model.AccessToken = null;
            model.AccessTokenType = null;
            model.AccessTokenExpiresIn = null;
            seoAccount.UpdateService(model);
            return Json(true);
        }
        #endregion

        #endregion

        #region Analytics
        public ActionResult Analytics()
        {
            ViewBag.SeoAccounts = seoAccount.GetAll();
            return View();
        }

        public ActionResult GoogleSpeedInsights()
        {
            return View();
        }
        #endregion
    }
}