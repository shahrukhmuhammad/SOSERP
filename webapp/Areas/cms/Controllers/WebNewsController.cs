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
using WebApp.Hubs;

namespace WebApp.Areas.CMS.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All, AppPermission.ViewCMS, AppPermission.CMS)]
    public class WebNewsController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private ICmsNews webNews;
        private IAppLog appLog;

        public WebNewsController()
        {
            webNews = db.As<ICmsNews>();
            appLog = db.As<IAppLog>();
        }

        #region News
        public ActionResult Index()
        {
            return View(webNews.GetAll());
        }

        public ActionResult Record(Guid? Id)
        {
            var model = new CmsNews();
            if (Id.HasValue)
            {
                model = webNews.GetById(Id.Value);
            }
            else
            {
                model.DateTime = DateTime.UtcNow;
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Record(CmsNews model)
        {
            try
            {
                model.UpdatedByUserId = CurrentUser.Id;
                if (model.Id.IsEmpty())
                {
                    model.CreatedByUserId = CurrentUser.Id;
                    webNews.Create(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebNews", "New news created", "~/CMS/WebNews/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>New news created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateCmsNews("News has been created successfully.");

                    TempData["SuccessMsg"] = "News has been created successfully.";
                }
                else
                {
                    webNews.Update(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebNews", "News updated", "~/CMS/WebNews/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>News updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateCmsNews("News has been updated successfully.");

                    TempData["SuccessMsg"] = "News has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebNews", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebNews/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Delete(Guid Id)
        {
            try
            {
                webNews.Delete(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebNews", "News deleted", "~/CMS/WebNews/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>News deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsNews("News has been deleted successfully.");

                TempData["SuccessMsg"] = "News has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebNews", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebNews/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteMultiple(string Ids)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        webNews.Delete(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebNews", "Multiple news deleted", "~/CMS/WebNews/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple news deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsNews("Multiple news has been deleted successfully.");

                TempData["SuccessMsg"] = "Selected news has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebNews", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebNews/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult ChangeNewsStatus(Guid Id)
        {
            webNews.ChangeStatus(Id, CurrentUser.Id);

            realtime.UpdateCmsNews("Status of the news has been changed successfully.");
            TempData["SuccessMsg"] = "Status of the news has been changed successfully.";
            return Json(true);
        }
        #endregion

        #region Json Requests
        public JsonResult GetAllNews()
        {
            var cmsNews = webNews.GetAll();
            return Json(cmsNews, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}