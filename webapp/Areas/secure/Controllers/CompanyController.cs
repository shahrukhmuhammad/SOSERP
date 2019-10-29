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

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize(AppPermission.All)]
    public class CompanyController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();
        private IAppLog appLog;
        private IAppNotification notify;
        private IOffice officeRepo;
        private IAppUser appUserRepo;

        public CompanyController()
        {
            appLog = db.As<IAppLog>();
            notify = db.As<IAppNotification>();
            officeRepo = db.As<IOffice>();
            appUserRepo = db.As<IAppUser>();
        }

        public ActionResult Index()
        {
            var allOffices = officeRepo.GetAll();
            foreach (var x in allOffices)
            {
                x.Contact = appUserRepo.GetUserById(x.ContactId);
            }
            ViewBag.AllOffices = allOffices;
            return View();
        }

        #region Company Settings
        public ActionResult Settings()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Settings(FormCollection form, HttpPostedFileBase logo, HttpPostedFileBase favicon, string returnUrl)
        {
            var logoPath = Server.MapPath("~/Content/Images/");
            var faviconPath = Server.MapPath("~/");

            try
            {
                if (logo.HasValue())
                {
                    logo.SaveAs(logoPath + "applogo.png");
                }

                if (favicon.HasValue())
                {
                    favicon.SaveAs(faviconPath + "appfavicon.ico");
                }


                var settings = form.AllKeys.ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Company", "Company settings updated", "~/Secure/Company/Settings > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Company settings updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Company settings have been saved successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Company", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Company/Settings > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
        }
        
        public ActionResult CompanyProfile()
        {
            return View();
        }
        
        #endregion
    }
}