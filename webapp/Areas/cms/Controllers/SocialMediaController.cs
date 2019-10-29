using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Linq;
using System.Web.Mvc;
using Insight.Database;
using CMS.Logic;
using CMS.Entity;
using WebApp.Hubs;

namespace WebApp.Areas.CMS.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All, AppPermission.ViewCMS, AppPermission.CMS)]
    public class SocialMediaController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private ICmsSocialMedia socialMedia;
        private IAppLog appLog;

        public SocialMediaController()
        {
            socialMedia = db.As<ICmsSocialMedia>();
            appLog = db.As<IAppLog>();
        }

        public ActionResult Index(Guid? Id)
        {
            if (Id.HasValue)
            {
                ViewBag.SocialMedia = socialMedia.GetById(Id.Value);
                return View(socialMedia.GetAll().Where(x => x.Id != Id.Value).ToList());
            }
            else
            {
                ViewBag.SocialMedia = new CmsSocialMedia();
                return View(socialMedia.GetAll());
            }
        }

        [HttpPost]
        public ActionResult Index(CmsSocialMedia model)
        {
            try
            {
                if (model.Id.IsEmpty())
                {
                    socialMedia.Create(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Social Media", "Social media created", "~/CMS/SocialMedia/Index > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Social media created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateCmsSocialMediaIcons("Social media icon has been created.");

                    TempData["SuccessMsg"] = "Social media icon has been created successfully.";
                }
                else
                {
                    socialMedia.Update(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Social Media", "Social media updated", "~/CMS/SocialMedia/Index > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Social media updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateCmsSocialMediaIcons("Social media icon has been updated.");

                    TempData["SuccessMsg"] = "Social media icon has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - Social Media", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/SocialMedia/Index > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Index", new { Id = "" });
        }

        [HttpPost]
        public JsonResult Delete(Guid Id)
        {
            try
            {
                socialMedia.Delete(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Social Media", "Social Media deleted", "~/CMS/SocialMedia/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Social media deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsSocialMediaIcons("Social media icon has been deleted.");

                TempData["SuccessMsg"] = "Social media icon has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - Social Media", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/SocialMedia/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                        socialMedia.Delete(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Social Media", "Multiple items deleted", "~/CMS/SocialMedia/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple items deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsSocialMediaIcons("Multiple social media icons has been deleted.");

                TempData["SuccessMsg"] = "Selected items has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - Social Media", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/SocialMedia/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        #region Json Requests
        public JsonResult GetAllSocialMediaIcons()
        {
            var model = socialMedia.GetAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}