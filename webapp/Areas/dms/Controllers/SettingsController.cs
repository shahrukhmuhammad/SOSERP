using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Web.Mvc;
using WebApp.Hubs;
using Insight.Database;

namespace WebApp.Areas.DMS.Controllers
{
    [AppAuthorize(AppPermission.All)]
    public class SettingsController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppUser appUserRepo;
        private IAppLog appLog;
        private IAppNotification notify;
        private IBenefitType btRepo;

        public SettingsController()
        {
            appUserRepo = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            notify = db.As<IAppNotification>();
            btRepo = db.As<IBenefitType>();
        }

        #region DMS
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Index(FormCollection form)
        {
            try
            {
                //AppModule.Update("DMS", dmsStatus, dmsMessage);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Company Drive Module Updated", "~/Secure/Setting/DMS > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Company Drive module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                if (AppSettings.GetVal<bool>("notification:Email"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        Emailer.Send(x.Email, "Company Drive module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Company Drive Module Updated");
                    }
                }
                if (AppSettings.GetVal<bool>("notification:Notify"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Company Drive Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/DMS", "Company Drive module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    }
                }

                realtime.UpdateNotifications("Company Drive module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "Company Drive has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Setting", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/DMS > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return View();
        }

        #endregion
    }
}