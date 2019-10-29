using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using BaseApp.Entity;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace WebApp.Areas.Operations.Controllers
{
    [AppAuthorize]
    public class AccountController : AppController
    {
        private IAppNotification notify;
        private IAppLog appLog;
        private IAppUser appUser;
        private IOffice ofcRepo;

        private IExtraFieldSection extras;

        public AccountController()
        {
            notify = db.As<IAppNotification>();
            appLog = db.As<IAppLog>();
            appUser = db.As<IAppUser>();
            ofcRepo = db.As<IOffice>();

            extras = db.As<IExtraFieldSection>();
        }

        #region Login / Logout / Recovery
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                IdentitySignout();
            }
            return View();
        }
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password, string returnUrl, string rememberMeTime, bool rememberMe = false)
        {
            try
            {
                var user = appUser.GetEmployeeByLogin(Username, Password);

                var locationData = "The account was accessed from <strong>" + Request.UserHostAddress + "</strong><br />";
                locationData += "The browser used for accessing the account is <strong>" + Request.Browser.Type + "</strong> at <strong>" + DateTime.UtcNow + "</strong><br />";
                locationData += "If it was not you, please change your security imediately by <a href='/Secure/Account/Security'>clicking here</a>.";

                if (user == null)
                {
                    #region Activity Log
                    appLog.Create(null, null, null, AppLogType.Activity, "Account", "Login Error", "~/Operations/Account/Login > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Invalid username or password for username: <strong>" + Username + "</strong>. <br />" + locationData + "</td></tr></table>");
                    #endregion
                    TempData["ErrorMsg"] = "Invalid username or password.";
                    return View();
                }
                IdentitySignin(user, rememberMeTime, null, rememberMe);

                var loginMetadata = "<strong>" + user.FullName + "</strong> logged in to the system.<br />";
                loginMetadata += locationData;
                

                var desc = "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'>";
                desc += "<tr><th class='text-center'>Description</th></tr><tr><td>";
                desc += loginMetadata;
                desc += "</td></tr></table>";

                #region Activity Log
                appLog.Create(user.OfficeId, null, user.Id, AppLogType.Activity, "Account", "Login", "~/Operations/Account/Login > HttpPost", desc);
                #endregion
                TempData["SuccessMsg"] = "You have been successfully logged in to the system.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(null, null, null, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Operations/Account/Login > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion
                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index", "Account", new { Area = "Operations" }) : RedirectToLocal(returnUrl);
        }

        public ActionResult Logout()
        {
            try
            {
                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Account", "Logout", "~/Operations/Account/Logout", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + CurrentUser.FullName + "</strong> logged out of the system.</td></tr></table>");
                #endregion

                IdentitySignout();

                TempData["SuccessMsg"] = "You have been successfully logged out of the system.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(null, null, CurrentUser.Id, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Operations/Account/Logout", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Redirect("Login");
        }


        #region Signin & Signout Functions
        public void IdentitySignin(AppUser appUser, string rememberMeTime, string providerKey = null, bool isPersistent = false)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, string.Format("{0} {1} {2}", appUser.FirstName, appUser.MiddleName, appUser.LastName)));
            claims.Add(new Claim(ClaimTypes.Sid, appUser.Id.ToString()));


            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            if (isPersistent)
            {
                if (rememberMeTime == "1 week")
                {
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) }, identity);
                }
                else if (rememberMeTime == "2 weeks")
                {
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14) }, identity);
                }
                else
                {
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
                }
            }
            else
            {
                AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
            }
        }

        public void IdentitySignout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
        #endregion
        #endregion


        public ActionResult Index()
        {
            var model = appUser.GetUserById(CurrentUser.Id);
            var extraFieldSection = extras.GetByModule("Employee");
            foreach (var x in extraFieldSection)
            {
                x.Fields = extras.GetByParentId(x.Id);
            }
            ViewBag.Extras = extraFieldSection;

            if (model.OfficeId.HasValue)
            {
                model.Office = ofcRepo.GetById(model.OfficeId.Value);
                if (model.Office != null)
                {
                    model.Office.Contact = appUser.GetUserById(model.Office.ContactId);
                }
            }

            model.Extras = appUser.GetExtrasByAppUserId(model.Id);
            return View(model);
        }


    }
}