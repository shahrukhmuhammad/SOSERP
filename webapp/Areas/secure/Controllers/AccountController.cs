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
using WebApp.Hubs;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize]
    public class AccountController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppNotification notify;
        private IAppLog appLog;
        private IAppUser appUser;
        private IOffice ofcRepo;
        private IAppRole appRole;

        public AccountController()
        {
            notify = db.As<IAppNotification>();
            appLog = db.As<IAppLog>();
            appUser = db.As<IAppUser>();
            ofcRepo = db.As<IOffice>();
            appRole = db.As<IAppRole>();
        }

        #region Profile Actions
        public ActionResult Index()
        {
            try
            {
                #region Actions
                var user = appUser.GetUserById(CurrentUser.Id);
                user.Role = appRole.GetById(user.RoleId);
                ViewBag.UpdatedUser = appUser.GetUserById(user.UpdatedByUserId);
                ViewBag.CreatedUser = appUser.GetUserById(user.CreatedByUserId);
                #endregion

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Account", "Profile viewed", "~/Secure/Account/Index", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + user.FullName + "</strong> viewed their profile.</td></tr></table>");
                #endregion

                return View(user);
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Account/Index", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion
                
                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";

                return RedirectToAction(Url.Action("Error", "AppLog", new { Area = "Secure" }));
            }
        }

        public ActionResult Edit()
        {
            try
            {
                var user = appUser.GetUserById(CurrentUser.Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Account", "Edit profile", "~/Secure/Account/Edit", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + user.FullName + "</strong> is goind to edit their profile.</td></tr></table>");
                #endregion

                return View(user);
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Account/Edit", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion
                
                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";

                return RedirectToAction(Url.Action("Error", "AppLog", new { Area = "Secure" }));
            }
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(AppUser model, HttpPostedFileBase file)
        {
            try
            {
                #region Actions
                var dpPath = Server.MapPath("~/Content/Uploads/Dp/");
                if (file.HasValue())
                {
                    file.SaveAs(dpPath + CurrentUser.Id + ".jpg");
                }
                model.Id = CurrentUser.Id;
                model.UpdatedByUserId = CurrentUser.Id;
                appUser.Update(model);
                #endregion

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Account", "Edit profile", "~/Secure/Account/Edit > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.FullName + "</strong> edited their profile.</td></tr></table>");
                #endregion
                
                TempData["SuccessMsg"] = "Account has been successfully updated.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Account/Edit > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Redirect("Index");
        }

        public ActionResult Security()
        {
            var model = appUser.GetUserById(CurrentUser.Id);
            return View(model);
        }
        
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            try
            {
                if (appUser.ChangePassword(CurrentUser.Id, OldPassword, NewPassword))
                {
                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Account", "Change password", "~/Secure/Account/ChangePassword > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + CurrentUser.FullName + "</strong> changed their password.</td></tr></table>");
                    #endregion

                    #region Notifications
                    if (AppSettings.GetVal<bool>("notification:Email"))
                    {
                        var model = appUser.GetUserById(CurrentUser.Id);
                        Emailer.Send(CurrentUser.Email, EmailTemplateType.PasswordChange, model);
                    }
                    if (AppSettings.GetVal<bool>("notification:Notify"))
                    {
                        notify.Create(CurrentUser.OfficeId, CurrentUser.Id, CurrentUser.Id, AppNotificationType.Warning, "Password change", AppSettings.GetVal("notification:URL") + "/Secure/Account/Security/", "Password changed by the logged in user from " + Request.UserHostAddress + ".");
                    }
                    realtime.UpdateNotifications("Password changed by the logged in user from " + Request.UserHostAddress + ".");
                    #endregion

                    TempData["SuccessMsg"] = "Password has been changed successfully.";
                }
                else
                {
                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Account", "Change password error", "~/Secure/Account/ChangePassword > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + CurrentUser.FullName + "</strong> has entered wrong old password.</td></tr></table>");
                    #endregion

                    TempData["ErrorMsg"] = "Invalid old password.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Account/ChangePassword > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion
                
                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Redirect("Security");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateQA(string Question1, string Answer1, string Question2, string Answer2)
        {
            try
            {
                appUser.UpdateQA(CurrentUser.Id, Question1, Answer1, Question2, Answer2);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Account", "Updated security questions answers", "~/Secure/Account/UpdateQA > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + CurrentUser.FullName + "</strong> updated their security question answers.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Changes has been successfully saved.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Account/UpdateQA > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Redirect("Security");
        }
        #endregion


        #region Recovery
        [AllowAnonymous]
        public ActionResult Recovery()
        {
            return View();
        }
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Recovery(string FirstName, string MiddleName, string LastName, string Email, string Question, string Answer)
        {
            try
            {
                var user = new AppUser();

                if (!string.IsNullOrEmpty(Email))
                {
                    user = appUser.UsernameRecoveryByEmail(Email, FirstName, MiddleName, LastName);
                }
                if (!string.IsNullOrEmpty(Question) && !string.IsNullOrEmpty(Answer))
                {
                    user = appUser.UsernameRecoveryByQA(Question, Answer, FirstName, MiddleName, LastName);
                }

                if (user == null)
                {
                    #region Activity Log
                    appLog.Create(null, null, null, AppLogType.Activity, "Account", "Username Recovery", "~/Secure/Account/Recovery > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Given details not matched with any record of Email:<strong>" + Email + "</strong>, First Name:<strong>" + FirstName + "</strong>, Middle Name:<strong>" + MiddleName + "</strong> and Last Name:<strong>" + LastName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["ErrorMsg"] = "Given details not matched with any record.";
                }
                else
                {
                    Emailer.Send(user.Email, EmailTemplateType.UsernameRecovery, user);

                    #region Activity Log
                    appLog.Create(user.OfficeId, user.Id, user.Id, AppLogType.Activity, "Account", "Username Recovery", "~/Secure/Account/Recovery > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Instructions for username recovery has been sent to email:<strong>"+Email+"</strong>. The search has been made against Email:<strong>" + Email + "</strong>, First Name:<strong>" + FirstName + "</strong>, Middle Name:<strong>" + MiddleName + "</strong> and Last Name:<strong>" + LastName + "</strong>. This request has been made from <strong>" + Request.UserHostAddress + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = "Instructions for username recovery has been sent to given email.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(null, null, null, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Account/Recovery > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request. We have informed the administrator, please check back later.";
            }

            return Redirect("Recovery");
        }


        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult PasswordRecovery(string Email, string Username)
        {
            try
            {
                var user = appUser.PasswordRecovery(Email, Username);
                if (user == null)
                {
                    #region Activity Log
                    appLog.Create(null, null, null, AppLogType.Activity, "Account", "Password Recovery", "~/Secure/Account/PasswordRecovery > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Given details not matched with any record of username <strong>" + Username + "</strong> and email <strong>" + Email + "</strong>.</td></tr></table>");
                    #endregion
                    
                    TempData["ErrorMsg"] = "Given details not matched with any record.";
                }
                else
                {
                    Emailer.Send(Email, EmailTemplateType.PasswordRecovery, user);

                    #region Activity Log
                    appLog.Create(user.OfficeId, user.Id, user.Id, AppLogType.Activity, "Account", "Password Recovery", "~/Secure/Account/PasswordRecovery > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Instructions for password recovery of username: <strong>" + Username + "</strong> sent to the email address <strong>" + Email + "</strong>. This request has been made from <strong>" + Request.UserHostAddress + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = "Instructions for password recovery has been sent to given email.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(null, null, null, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Account/PasswordRecovery > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion
                
                TempData["ErrorMsg"] = "We have encountered an error while processing your request. We have informed the administrator, please check back later.";
            }

            return Redirect("Recovery");
        }
        #endregion


        #region Login & Logout
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                TempData["ErrorMsg"] = "You are already logged in to the system.";
                return RedirectToAction("Index", "Dashboard", new { Area = "Dashboard" });
            }
            return View();
        }
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password, string returnUrl, string rememberMeTime, bool rememberMe = false)
        {
            try
            {
                var user = appUser.GetUserByLogin(Username, Password);

                if (user == null)
                {
                    #region Activity Log
                    appLog.Create(null, null, null, AppLogType.Activity, "Account", "Login Error", "~/Secure/Account/Login > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Invalid username or password for username: <strong>" + Username + "</strong>.</td></tr></table>");
                    #endregion
                    TempData["ErrorMsg"] = "Invalid username or password.";
                    return View();
                }
                IdentitySignin(user, rememberMeTime, null, rememberMe);

                var loginMetadata = "<strong>" + user.FullName + "</strong> logged in to the system.<br />";
                loginMetadata += "The account was accessed from <strong>" + Request.UserHostAddress + "</strong><br />";
                loginMetadata += "The browser used for accessing the account is <strong>" + Request.Browser.Type + "</strong> at <strong>" + DateTime.UtcNow + "</strong><br />";
                loginMetadata += "If it was not you, please change your security imediately by <a href='/Secure/Account/Security'>clicking here</a>.";

                var desc = "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'>";
                desc += "<tr><th class='text-center'>Description</th></tr><tr><td>";
                desc += loginMetadata;
                desc += "</td></tr></table>";

                #region Notifications
                if (AppSettings.GetVal<bool>("notification:Email"))
                {
                    Emailer.Send(user.Email, desc, "Your account has been recently accessed.");
                }
                if (AppSettings.GetVal<bool>("notification:Notify"))
                {
                    notify.Create(user.OfficeId, user.Id, user.Id, AppNotificationType.Warning, "Your account has been recently accessed.", AppSettings.GetVal("notification:URL") + "/Secure/Account/Security/", loginMetadata);
                }
                realtime.UpdateNotifications("Your account has been recently accessed.");
                #endregion

                #region Activity Log
                appLog.Create(user.OfficeId, null, user.Id, AppLogType.Activity, "Account", "Login", "~/Secure/Account/Login > HttpPost", desc);
                #endregion
                TempData["SuccessMsg"] = "You have been successfully logged in to the system.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(null, null, null, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Account/Login > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion
                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index", "Dashboard", new { Area = "Dashboard" }) : RedirectToLocal(returnUrl);
        }

        public ActionResult Logout()
        {
            try
            {
                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Account", "Logout", "~/Secure/Account/Logout", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + CurrentUser.FullName + "</strong> logged out of the system.</td></tr></table>");
                #endregion

                IdentitySignout();

                TempData["SuccessMsg"] = "You have been successfully logged out of the system.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(null, null, CurrentUser.Id, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Account/Logout", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion
                
                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Redirect("Login");
        }
        #endregion


        #region Profile Actions
        [HttpPost]
        public JsonResult CheckDuplicateUsername(Guid Id, string Username)
        {
            var checkUsername = appUser.CheckDuplicateUsername(Id, Username);
            if (checkUsername == null)
            {
                return Json(true);
            }
            else
            {
                return Json("Username already exists.");
            }
        }

        [HttpPost]
        public JsonResult ChangeTheme(string ThemeName)
        {
            try
            {
                appUser.ChangeTheme(CurrentUser.Id, ThemeName);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Account", "Theme Changed", "~/Secure/Account/ChangeTheme > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Theme changed to <strong>" + ThemeName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Theme has been successfully changed.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Account", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Account/ChangeTheme > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion


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
    }
}