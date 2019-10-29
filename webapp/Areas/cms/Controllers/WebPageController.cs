using BaseApp.Entity;
using BaseApp.System;
using CMS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using CMS.Entity;
using BaseApp.Logic;
using WebApp.Hubs;

namespace WebApp.Areas.CMS.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All, AppPermission.ViewCMS, AppPermission.CMS)]
    public class WebPageController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppUser appUser;
        private ICmsPage webPage;
        private IAppLog appLog;

        public WebPageController()
        {
            appUser = db.As<IAppUser>();
            webPage = db.As<ICmsPage>();
            appLog = db.As<IAppLog>();
            ViewBag.AllPages = webPage.GetAll();
        }

        public ActionResult Index()
        {
            var pages = webPage.GetAll().Where(x => x.IsSystem == false).ToList();
            foreach (var x in pages)
            {
                x.ChildPages = webPage.GetByParentId(x.Id);
            }
            return View(pages);
        }

        #region Page Steps

        #region Step1
        public ActionResult Step1(Guid? Id)
        {
            var model = new CmsPage();
            if (Id.HasValue)
            {
                model = webPage.GetById(Id.Value);
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Step1(CmsPage model)
        {
            try
            {
                if (model.Id.IsEmpty())
                {
                    model.CreatedByUserId = model.UpdatedByUserId = CurrentUser.Id;
                    model.Id = webPage.Create(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page created", "~/CMS/WebPage/Step1 > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> page created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = "New page has been created successfully.";
                }
                else
                {
                    model.UpdatedByUserId = CurrentUser.Id;
                    webPage.Update(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page updated", "~/CMS/WebPage/Step1 > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> page updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = "Page has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/Step1 > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Step2", new { Id = model.Id });
        }
        #endregion

        #region Step2
        public ActionResult Step2(Guid Id)
        {
            return View(webPage.GetById(Id));
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step2(CmsPage model)
        {
            try
            {
                webPage.UpdateContent(model.Id, model.Contents, CurrentUser.Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page created", "~/CMS/WebPage/Step2 > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Page content updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Page content has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/Step2 > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return RedirectToAction("Step3", new { Id = model.Id });
        }
        #endregion

        #region Step3
        public ActionResult Step3(Guid Id)
        {
            return View(webPage.GetById(Id));
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Step3(CmsPage model)
        {
            try
            {
                model.UpdatedByUserId = CurrentUser.Id;
                webPage.UpdateMetadata(model);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page SEO", "~/CMS/WebPage/Step3 > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Page seo updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Page SEO has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/Step3 > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Step4", new { Id = model.Id });
        }
        #endregion

        #region Step4
        public ActionResult Step4(Guid Id)
        {
            var model = webPage.GetById(Id);
            model.UpdatedByUser = appUser.GetUserById(model.UpdatedByUserId);
            model.CreatedByUser = appUser.GetUserById(model.CreatedByUserId);
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Step4(Guid Id, string submit)
        {
            var model = webPage.GetById(Id);
            try
            {
                if (submit == "draft")
                {
                    webPage.ChangeStatus(Id, CmsPageStatus.Draft, CurrentUser.Id);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page saved as draft", "~/CMS/WebPage/Step4 > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Page saved as <strong>Draft</strong> by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = "Page has been saved as draft successfully.";
                }

                if (submit == "publish")
                {
                    webPage.ChangeStatus(Id, CmsPageStatus.Published, CurrentUser.Id);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page is published", "~/CMS/WebPage/Step4 > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Page is <strong>Published</strong> by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = "Page has been published successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/Step4 > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return (model.IsSystem ? RedirectToAction("Step1", new { Id = model.Id }) : RedirectToAction("Index"));
        }
        #endregion

        #endregion


        #region Page Record
        public ActionResult Record(Guid? Id)
        {
            var model = new CmsPage();
            if (Id.HasValue)
            {
                model = webPage.GetById(Id.Value);
                model.PageSections = webPage.GetSectionsByPageId(model.Id);
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Record(CmsPage model, string submit)
        {
            try
            {
                if (model.Id.IsEmpty())
                {
                    if (submit == "draft")
                    {
                        model.Status = CmsPageStatus.Draft;
                    }
                    if (submit == "publish")
                    {
                        model.Status = CmsPageStatus.Published;
                    }

                    model.CreatedByUserId = model.UpdatedByUserId = CurrentUser.Id;
                    webPage.Create(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page created", "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> page created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateCmsPages("New page has been created successfully.");

                    TempData["SuccessMsg"] = "New page has been created successfully.";
                }
                else
                {
                    if (submit == "draft")
                    {
                        model.Status = CmsPageStatus.Draft;
                    }
                    if (submit == "publish")
                    {
                        model.Status = CmsPageStatus.Published;
                    }

                    model.UpdatedByUserId = CurrentUser.Id;
                    webPage.Update(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page updated", "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> page updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateCmsPages("Page has been updated successfully.");
                    TempData["SuccessMsg"] = "Page has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return (model.IsSystem ? RedirectToAction("Record", new { Id = model.Id }) : RedirectToAction("Index"));
        }
        #endregion

        #region Footer

        public ActionResult Footer()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Footer(FormCollection form)
        {
            try
            {
                var settings = form.AllKeys.ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "CMS - WebPage", "Website footer updated", "~/CMS/WebPage/Footer > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Website footer updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Footer has been saved successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "CMS - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/Footer > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Footer");
        }

        #endregion

        #region Remote Actions
        
        [HttpPost]
        public JsonResult CheckUniqueSlug(Guid Id, string Slug)
        {
            if (webPage.UniqueSlug(Id, Slug))
            {
                return Json(true);
            }
            else
            {
                return Json("Url already exists.");
            }
        }

        [HttpPost]
        public JsonResult DeletePage(Guid Id)
        {
            try
            {
                webPage.Delete(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page deleted", "~/CMS/WebPage/DeletePage > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Page deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsPages("Web page has been deleted successfully.");

                TempData["SuccessMsg"] = "Web page has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/DeletePage > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteMultiplePages(string Ids)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        webPage.Delete(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Multiple pages deleted", "~/CMS/WebPage/DeleteMultipleSections > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple pages deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsPages("Multiple pages deleted.");

                TempData["SuccessMsg"] = "Selected pages deleted.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/DeleteMultipleSections > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult ChangePageStatus(Guid Id)
        {
            try
            {
                var page = webPage.GetById(Id);

                if (page.Status == CmsPageStatus.Draft)
                {
                    webPage.ChangeStatus(Id, CmsPageStatus.Published, CurrentUser.Id);
                }
                else
                {
                    webPage.ChangeStatus(Id, CmsPageStatus.Draft, CurrentUser.Id);
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page status updated", "~/CMS/WebPage/ChangePageStatus > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Page status changed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsPages("Status of the web page has been changed successfully.");

                TempData["SuccessMsg"] = "Status of the web page has been changed successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/ChangePageStatus > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        #endregion

        #region Page Sections
        public ActionResult Section(Guid? Id, Guid? p)
        {
            var model = new CmsPageSection();
            if (Id.HasValue)
            {
                model = webPage.GetSectionById(Id.Value);
                model.Page = webPage.GetById(model.PageId);
                model.Page.PageSections = webPage.GetSectionsByPageId(model.PageId);
            }
            else
            {
                if (p.HasValue)
                {
                    model.Page = webPage.GetById(p.Value);
                    model.Page.PageSections = webPage.GetSectionsByPageId(p.Value);
                }
                else
                {
                    TempData["ErrorMsg"] = "No page found.";
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Section(CmsPageSection model, string returnUrl)
        {
            try
            {
                if (model.Id.IsEmpty())
                {
                    model.Id = webPage.CreateSection(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "New section created", "~/CMS/WebPage/Section > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>New section created in <trong>" + model.Page.Name + "</strong> by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = "New section has been created successfully.";
                }
                else
                {
                    webPage.UpdateSection(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", model.Name + " section updated", "~/CMS/WebPage/Section > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>" + model.Name + " section updated in <trong>" + model.Page.Name + "</strong> by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = "Section has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/Section > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index") : RedirectToLocal(returnUrl);
        }

        [HttpPost]
        public JsonResult ChangeSectionStatus(Guid Id)
        {
            try
            {
                webPage.ChangeSectionStatus(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page section's status updated", "~/CMS/WebPage/ChangeSectionStatus > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Page section's status changed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Status of the web page section has been changed successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/ChangeSectionStatus > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult ChangeMultipleSectionsStatus(string Ids)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        webPage.ChangeSectionStatus(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Multiple sections' status changed", "~/CMS/WebPage/ChangeMultipleSectionsStatus > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple page sections' status updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Selected page sections's status updated.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/ChangeMultipleSectionsStatus > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteMultipleSections(string Ids)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        webPage.DeleteSection(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Multiple sections deleted", "~/CMS/WebPage/DeleteMultipleSections > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple page sections deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Selected page sections deleted.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/DeleteMultipleSections > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteSection(Guid Id)
        {
            try
            {
                webPage.DeleteSection(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page section deleted", "~/CMS/WebPage/DeleteSection > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Page section deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Page section deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/DeleteSection > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Json Requests
        public JsonResult GetAllPages()
        {
            var pages = webPage.GetAll().Where(x => x.IsSystem == false).ToList();
            foreach (var x in pages)
            {
                x.ChildPages = webPage.GetByParentId(x.Id);
            }
            return Json(pages, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}