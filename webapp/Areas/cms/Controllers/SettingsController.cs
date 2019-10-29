using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Hubs;
using Insight.Database;

namespace WebApp.Areas.CMS.Controllers
{
    [AppAuthorize(AppPermission.All)]
    public class SettingsController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppUser appUserRepo;
        private IAppLog appLog;
        private IAppNotification notify;

        public SettingsController()
        {
            appUserRepo = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            notify = db.As<IAppNotification>();
        }

        #region CMS Settings
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Index(FormCollection form, HttpPostedFileBase logo, HttpPostedFileBase favicon, string returnUrl)
        {
            var logoPath = Server.MapPath("~/Content/Images/");
            var faviconPath = Server.MapPath("~/");

            try
            {
                if (logo.HasValue())
                {
                    logo.SaveAs(logoPath + "logo.png");
                }

                if (favicon.HasValue())
                {
                    favicon.SaveAs(faviconPath + "favicon.ico");
                }


                var settings = form.AllKeys.Where(x => x != "cmsMessage" && x != "cmsStatus" && x != "returnUrl").ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                //AppModule.Update("CMS", cmsStatus, cmsMessage);


                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Content Management Module Updated", "~/CMS/Settings > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Content management module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                if (AppSettings.GetVal<bool>("notification:Email"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        Emailer.Send(x.Email, "Content Management module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Content Management Module Updated");
                    }
                }
                if (AppSettings.GetVal<bool>("notification:Notify"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Content Management Module Updated", AppSettings.GetVal("notification:URL") + "/CMS/Settings", "Content Management module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    }
                }

                realtime.UpdateNotifications("Content Management module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "Content Management has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Setting", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Settings/CMS > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
        }
        #endregion
    }
}