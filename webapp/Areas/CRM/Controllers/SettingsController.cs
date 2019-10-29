using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Web.Mvc;
using WebApp.Hubs;
using Insight.Database;
using System.Linq;

namespace WebApp.Areas.CRM.Controllers
{
    [AppAuthorize(AppPermission.All)]
    public class SettingsController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppUser appUserRepo;
        private IAppLog appLog;
        private IAppNotification notify;
        private IBenefitType btRepo;
        private IAppSMTP appSmtp;

        public SettingsController()
        {
            appUserRepo = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            notify = db.As<IAppNotification>();
            btRepo = db.As<IBenefitType>();
            appSmtp = db.As<IAppSMTP>();
        }

        #region CRM
        public ActionResult Index()
        {
            return View(appSmtp.GetAll());
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Index(FormCollection form, string returnUrl)
        {
            try
            {
                var settings = form.AllKeys.Where(x => x != "returnUrl").ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                //AppModule.Update("CRM", crmStatus, crmMessage);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "CRM Module Updated", "~/Secure/Setting/CRM > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>CRM module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                if (AppSettings.GetVal<bool>("notification:Email"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        Emailer.Send(x.Email, "CRM module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "CRM Module Updated");
                    }
                }
                if (AppSettings.GetVal<bool>("notification:Notify"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "CRM Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/CRM", "CRM module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    }
                }

                realtime.UpdateNotifications("CRM module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "CRM has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Setting", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/CRM > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
        }

        #region Remote Functions
        [HttpPost]
        public JsonResult SaveAutoInactive(bool Approval = false)
        {
            AppSettings.SetVal("Crm:AutoInactive", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveAccountSecurity(bool Approval = false)
        {
            AppSettings.SetVal("Crm:AccountSecurity", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveSecurityQuestions(bool Approval = false)
        {
            AppSettings.SetVal("Crm:SecurityQuestions", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveEmailCode(bool Approval = false)
        {
            AppSettings.SetVal("Crm:EmailCode", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveSMSCode(bool Approval = false)
        {
            AppSettings.SetVal("Crm:SMSCode", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        #endregion
        #endregion
    }
}