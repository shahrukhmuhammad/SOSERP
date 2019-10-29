using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Web.Mvc;
using Insight.Database;
using CMS.Logic;
using CMS.Entity;
using WebApp.Hubs;

namespace WebApp.Areas.CMS.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All, AppPermission.ViewCMS, AppPermission.CMS)]
    public class WebContentController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private ICmsContent webContent;
        private IAppLog appLog;

        public WebContentController()
        {
            webContent = db.As<ICmsContent>();
            appLog = db.As<IAppLog>();
        }

        #region Contents
        public ActionResult Index()
        {
            return View(webContent.GetAll());
        }

        public ActionResult Record(Guid? Id)
        {
            var model = new CmsContent();
            if (Id.HasValue)
            {
                model = webContent.GetById(Id.Value);
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Record(CmsContent model)
        {
            try
            {
                if (model.Id.IsEmpty())
                {
                    model.UpdatedByUserId = model.CreatedByUserId = CurrentUser.Id;
                    webContent.Create(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebContent", "New content created", "~/CMS/WebContent/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>"+model.Name+"</strong> content created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateCmsContents("New content has been created.");

                    TempData["SuccessMsg"] = "Content has been created successfully.";
                }
                else
                {
                    model.UpdatedByUserId = CurrentUser.Id;
                    webContent.Update(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebContent", "Content updated", "~/CMS/WebContent/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> content updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateCmsContents("Content has been updated.");

                    TempData["SuccessMsg"] = "Content has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebContent", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebContent/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                webContent.Delete(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebContent", "Content deleted", "~/CMS/WebContent/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Content deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsContents("Content has been deleted.");

                TempData["SuccessMsg"] = "Content has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebContent", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebContent/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                        webContent.Delete(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebContent", "Multiple contents deleted", "~/CMS/WebContent/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple contents deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsContents("Multiple contents has been deleted.");

                TempData["SuccessMsg"] = "Selected contents has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebContent", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebContent/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult CheckUniqueContentName(Guid Id, string Name)
        {
            if (webContent.UniqueContent(Id, Name))
            {
                return Json(true);
            }
            else
            {
                return Json("Name already exists.");
            }
        }

        [HttpPost]
        public JsonResult ChangeContentStatus(Guid Id)
        {
            try
            {
                webContent.ChangeStatus(Id, CurrentUser.Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebContent", "Content status updated", "~/CMS/WebContent/ChangeContentStatus > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Content status changed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsContents("Status of the content has been changed.");

                TempData["SuccessMsg"] = "Status of the content has been changed successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebContent", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebContent/ChangeContentStatus > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Json Requests
        public JsonResult GetAllContents()
        {
            var model = webContent.GetAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}