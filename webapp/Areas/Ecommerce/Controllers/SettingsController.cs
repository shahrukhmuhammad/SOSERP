using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Linq;
using System.Web.Mvc;
using WebApp.Hubs;
using Insight.Database;
using BaseApp;

namespace WebApp.Areas.Ecommerce.Controllers
{
    [AppAuthorize(AppPermission.All)]
    public class SettingsController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppUser appUserRepo;
        private IAppLog appLog;
        private IInsurance insuranceRepo;
        private IAppNotification notify;
        private IAppSMTP appSmtp;

        public SettingsController()
        {
            appUserRepo = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            insuranceRepo = db.As<IInsurance>();
            notify = db.As<IAppNotification>();
            appSmtp = db.As<IAppSMTP>();
        }

        #region ECommerce Settings
        public ActionResult Index()
        {
            return View(appSmtp.GetAll());
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Index(FormCollection form, string returnUrl)
        {

            try
            {
                var settings = form.AllKeys.Where(x => x != "returnUrl" && x != "shippingCompany-dataTable_length" && x != "shippingPackages-dataTable_length" && x != "shippingPostCodes-dataTable_length").ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                //AppModule.Update("Ecommerce", ecommerceStatus, ecommerceMessage);


                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Ecommerce Module Updated", "~/Ecommerce/Settings > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Ecommerce module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                if (AppSettings.GetVal<bool>("notification:Email"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        Emailer.Send(x.Email, "Ecommerce module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Ecommerce Module Updated");
                    }
                }
                if (AppSettings.GetVal<bool>("notification:Notify"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Ecommerce Module Updated", AppSettings.GetVal("notification:URL") + "/Ecommerce/Settings", "Ecommerce module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    }
                }

                realtime.UpdateNotifications("Ecommerce module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "Ecommerce has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Setting", ex.GetType().Name.ToSpacedTitleCase(), "~/Ecommerce/Setting > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
        }

        #endregion
    }
}