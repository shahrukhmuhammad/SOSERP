using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using BaseApp.Entity;
using BaseApp.System;
using BaseApp.Logic;
using WebApp.Hubs;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize(AppPermission.All, AppPermission.ViewContact, AppPermission.Contact)]
    public class AccessControlController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppUser appUser;
        private IAppLog appLog;
        private IOffice ofcRepo;
        private IAppNotification notify;
        private IAppRole appRole;
        public AccessControlController()
        {
            appUser = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            ofcRepo = db.As<IOffice>();
            notify = db.As<IAppNotification>();
            appRole = db.As<IAppRole>();

            ViewBag.AllOffices = ofcRepo.GetAll();
        }

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Index(Guid? Id)
        {
            try
            {
                var model = new List<AppUser>();
                if (Id.HasValue)
                {
                    var office = ofcRepo.GetById(Id.Value);
                    office.Contact = appUser.GetUserById(office.ContactId);
                    ViewBag.Office = office;

                    model = appUser.GetAllByOfficeId(Id.Value);
                    foreach (var x in model)
                    {
                        x.Role = appRole.GetById(x.RoleId);
                        if (x.OfficeId.HasValue)
                        {
                            x.Office = ofcRepo.GetById(x.OfficeId.Value);
                        }
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Access Control", office.Code + " " + office.Title + " Contacts Viewed", "~/Secure/AccessControl/Index", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + office.Code + " " + office.Title + "</strong> contacts viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion
                }
                else
                {
                    model = appUser.GetAll().Where(x => x.Id != CurrentUser.Id).ToList();
                    foreach (var x in model)
                    {
                        x.Role = appRole.GetById(x.RoleId);
                        if (x.OfficeId.HasValue)
                        {
                            x.Office = ofcRepo.GetById(x.OfficeId.Value);
                        }
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Access Control", "All contacts viewed", "~/Secure/AccessControl/Index", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>All contacts viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion
                }
                return View(model);
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/Index", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";

                return Redirect(Url.Action("Error", "AppLog", new { Area = "Secure" }));
            }
        }

        public ActionResult _ManageAccess(Guid Id)
        {
            ViewBag.AllRoles = appRole.GetAll().Where(x => x.IsSystem == false).ToList();
            var userDetails = new AppUser();
            var model = new AppUserPermissions();
            model.UserId = Id;
            userDetails = appUser.GetUserById(Id);
            model.RoleId = userDetails.RoleId;
            if (appRole.IsAlreadyExist(model.UserId) > 0)
            {
                model.Permissions = appRole.GetCustomPermissions(model.UserId);
            }
            else
            {
                model.Permissions = appRole.GetPermissionsById(userDetails.RoleId);
            }
            
            return PartialView(model);
        }

        public ActionResult AccessManagement(Guid Id)
        {
            ViewBag.AllRoles = appRole.GetAll().Where(x => x.IsSystem == false).ToList();
            var userDetails = new AppUser();
            var model = new AppUserPermissions();
            model.UserId = Id;
            userDetails = appUser.GetUserById(Id);
            model.RoleId = userDetails.RoleId;
            if (appRole.IsAlreadyExist(model.UserId) > 0)
            {
                model.Permissions = appRole.GetCustomPermissions(model.UserId);
            }
            else
            {
                model.Permissions = appRole.GetPermissionsById(userDetails.RoleId);
            }

            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult _ManageAccess(AppUserPermissions model, string returnUrl)
        {
            try
            {
                #region Compare Permissions
                if (model.Permissions != appRole.GetPermissionsById(model.RoleId))
                {
                    if (appRole.IsAlreadyExist(model.UserId) > 0)
                    {
                        appRole.DeleteByUser(model.UserId);
                        appRole.Create(model);
                    }
                    else
                    {
                        appRole.Create(model);
                    }
                }
                #endregion

                #region Activity Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Configuration", "Application Restarted", "~/Secure/Config/RestartApp > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Application restarted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                //if (AppSettings.GetVal<bool>("notification:Email"))
                //{
                //    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                //    {
                //        Emailer.Send(x.Email, "Application restarted by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Application Restarted");
                //    }
                //}
                //if (AppSettings.GetVal<bool>("notification:Notify"))
                //{
                //    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                //    {
                //        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Application Restarted", AppSettings.GetVal("notification:URL") + "/Secure/Config/Info", "Application restarted by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                //    }
                //}

                realtime.UpdateNotifications("Application restarted by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "Permissions have Granted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Configuration", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Config/RestartApp > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index", "AccessControl", new { Area = "Secure" }) : RedirectToLocal(returnUrl);
        }
        public ActionResult Contacts(Guid? Id)
        {
            try
            {
                var model = new List<AppUser>();
                if (Id.HasValue)
                {
                    var office = ofcRepo.GetById(Id.Value);
                    office.Contact = appUser.GetUserById(office.ContactId);
                    ViewBag.Office = office;

                    model = appUser.GetAllByOfficeId(Id.Value);
                    foreach (var x in model)
                    {
                        x.Role = appRole.GetById(x.RoleId);
                        if (x.OfficeId.HasValue)
                        {
                            x.Office = ofcRepo.GetById(x.OfficeId.Value);
                        }
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Access Control", office.Code + " " + office.Title + " Contacts Viewed", "~/Secure/AccessControl/Contacts", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + office.Code + " " + office.Title + "</strong> contacts viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion
                }
                else
                {
                    model = appUser.GetAll().Where(x => x.Id != CurrentUser.Id).ToList();
                    foreach (var x in model)
                    {
                        x.Role = appRole.GetById(x.RoleId);
                        if (x.OfficeId.HasValue)
                        {
                            x.Office = ofcRepo.GetById(x.OfficeId.Value);
                        }
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Access Control", "All contacts viewed", "~/Secure/AccessControl/Contacts", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>All contacts viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion
                }
                return View(model);
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/Contacts", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";

                return Redirect(Url.Action("Error", "AppLog", new { Area = "Secure" }));
            }
        }

        public ActionResult Details(Guid Id)
        {
            try
            {
                var user = appUser.GetUserById(Id);
                user.Role = appRole.GetById(user.RoleId);
                ViewBag.UpdatedUser = appUser.GetUserById(user.UpdatedByUserId);
                ViewBag.CreatedUser = appUser.GetUserById(user.CreatedByUserId);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Access Control", user.Code + " " + user.FullName + " Contact Viewed", "~/Secure/AccessControl/Details", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + user.Code + " " + user.FullName + "</strong> contact viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                return View(user);
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/Details", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";

                return Redirect(Url.Action("Error", "AppLog", new { Area = "Secure" }));
            }
        }

        [AppAuthorize(AppPermission.All, AppPermission.Contact)]
        public ActionResult Record(Guid? Id)
        {
            ViewBag.AllRoles = appRole.GetAll().Where(x => x.IsSystem == false).ToList();
            var model = new AppUser();
            if (Id.HasValue)
            {
                model = appUser.GetUserById(Id.Value);
                //model.Office = ofcRepo.GetById(model.OfficeId.Value);

                //ViewBag.Office = model.Office;
            }
            else
            {
                //if (o.HasValue)
                //{
                //    ViewBag.Office = ofcRepo.GetById(o.Value);
                //}

                model.DateOfBirth = DateTime.UtcNow;
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken, AppAuthorize(AppPermission.All, AppPermission.Contact)]
        public ActionResult Record(AppUser model, HttpPostedFileBase file)
        {
            try
            {
                if (model.Id.IsEmpty())
                {
                    model.Code = appUser.GetMaxCode();
                    model.Status = AppUserStatus.Active;
                    model.Password = Uuid.Random(8);
                    model.CreatedByUserId = model.UpdatedByUserId = CurrentUser.Id;
                    model.Id = appUser.Create(model);
                    Emailer.Send(model.Email, EmailTemplateType.UserSignup, model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Access Control", model.Code + " " + model.FullName + " Contact Created", "~/Secure/AccessControl/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.FullName + "</strong> contact created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region Notifications
                    if (AppSettings.GetVal<bool>("notification:Email"))
                    {
                        foreach (var x in appUser.GetByPermission(AppPermission.All))
                        {
                            Emailer.Send(x.Email, model.Code + " " + model.FullName + " contact created by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "New Contact Created");
                        }
                        foreach (var x in appUser.GetByPermission(AppPermission.Contact))
                        {
                            Emailer.Send(x.Email, model.Code + " " + model.FullName + " contact created by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "New Contact Created");
                        }
                    }
                    if (AppSettings.GetVal<bool>("notification:Notify"))
                    {
                        foreach (var x in appUser.GetByPermission(AppPermission.All))
                        {
                            notify.Create(model.OfficeId, x.Id, model.Id, AppNotificationType.Alert, "New contact created", AppSettings.GetVal("notification:URL") + "/Secure/AccessControl/Details/" + model.Id, model.Code + " " + model.FullName + " contact created by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }

                        foreach (var x in appUser.GetByPermission(AppPermission.Contact))
                        {
                            notify.Create(model.OfficeId, x.Id, model.Id, AppNotificationType.Alert, "New contact created", AppSettings.GetVal("notification:URL") + "/Secure/AccessControl/Details/" + model.Id, model.Code + " " + model.FullName + " contact created by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }
                    }
                    #endregion

                    realtime.UpdateAppUsers("New contact created.");

                    TempData["SuccessMsg"] = "Contact has been created successfully. An email has also been sent to the application user's email address.";
                }
                else
                {
                    model.UpdatedByUserId = CurrentUser.Id;
                    appUser.Update(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Access Control", model.Code + " " + model.FullName + " Contact Updated", "~/Secure/AccessControl/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.FullName + "</strong> contact updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateAppUsers("Contact has been updated.");

                    TempData["SuccessMsg"] = "Contact has been updated successfully.";
                }

                var dpPath = Server.MapPath("~/Content/Uploads/Dp/");
                if (file.HasValue())
                {
                    file.SaveAs(dpPath + model.Id + ".jpg");
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Index");
        }

        #region Roles
        [AppAuthorize(AppPermission.All)]
        public ActionResult Roles()
        {
            try
            {
                var model = appRole.GetAll();
                foreach (var x in model)
                {
                    x.Contacts = appUser.GetAllByRoleId(x.Id);
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Access Control", "All roles viewed", "~/Secure/AccessControl/Roles", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>All roles viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                return View(model);
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/Roles", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";

                return Redirect(Url.Action("Error", "AppLog", new { Area = "Secure" }));
            }
        }
        [HttpGet]
        public ActionResult _RolePermissions(Guid Id)
        {
            var model = new AppRole();
            model = appRole.GetById(Id);
            return PartialView(model);
        }

        [AppAuthorize(AppPermission.All)]
        public ActionResult Role(Guid? Id)
        {
            var model = new AppRole();
            if (Id.HasValue)
            {
                model = appRole.GetById(Id.Value);
            }
            return View(model);
        }
        [AppAuthorize(AppPermission.All), HttpPost, ValidateAntiForgeryToken]
        public ActionResult Role(AppRole model)
        {
            try
            {
                if (model.Id.IsEmpty())
                {
                    model.Code = appRole.GetMaxCode();
                    model.Id = appRole.Create(model);

                    #region Activity Log
                    appLog.Create(null, model.Id, CurrentUser.Id, AppLogType.Activity, "Access Control", model.Code + " " + model.Title + " Role Created", "~/Secure/AccessControl/Role > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.Title + "</strong> role created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region Notifications
                    if (AppSettings.GetVal<bool>("notification:Email"))
                    {
                        foreach (var x in appUser.GetByPermission(AppPermission.All))
                        {
                            Emailer.Send(x.Email, model.Code + " " + model.Title + " role created by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "New Role Created");
                        }
                    }
                    if (AppSettings.GetVal<bool>("notification:Notify"))
                    {
                        foreach (var x in appUser.GetByPermission(AppPermission.All))
                        {
                            notify.Create(null, x.Id, model.Id, AppNotificationType.Alert, "New role created", AppSettings.GetVal("notification:URL") + "/Secure/AccessControl/Roles", model.Code + " " + model.Title + " role created by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }
                    }
                    #endregion

                    realtime.UpdateAppUserRoles("New role has been created.");

                    TempData["SuccessMsg"] = "Role has been created successfully.";
                }
                else
                {
                    appRole.Update(model);

                    #region Activity Log
                    appLog.Create(null, model.Id, CurrentUser.Id, AppLogType.Activity, "Access Control", model.Code + " " + model.Title + " Role Updated", "~/Secure/AccessControl/Role > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.Title + "</strong> role updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region Notifications
                    if (AppSettings.GetVal<bool>("notification:Email"))
                    {
                        foreach (var x in appUser.GetByPermission(AppPermission.All))
                        {
                            Emailer.Send(x.Email, model.Code + " " + model.Title + " role updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Role Updated");
                        }
                    }
                    if (AppSettings.GetVal<bool>("notification:Notify"))
                    {
                        foreach (var x in appUser.GetByPermission(AppPermission.All))
                        {
                            notify.Create(null, x.Id, model.Id, AppNotificationType.Alert, "Role updated", AppSettings.GetVal("notification:URL") + "/Secure/AccessControl/Roles", model.Code + " " + model.Title + " role updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }
                    }
                    #endregion

                    realtime.UpdateAppUserRoles("Role has been updated.");

                    TempData["SuccessMsg"] = "Role has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/Role > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Roles");
        }

        [HttpPost, AppAuthorize(AppPermission.All)]
        public JsonResult DeleteRole(Guid Id)
        {
            try
            {
                var model = appRole.GetById(Id);

                #region Activity Log
                appLog.Create(null, model.Id, CurrentUser.Id, AppLogType.Activity, "Access Control", model.Code + " " + model.Title + " Role Deleted", "~/Secure/AccessControl/DeleteRole > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.Title + "</strong> role deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                if (AppSettings.GetVal<bool>("notification:Email"))
                {
                    foreach (var x in appUser.GetByPermission(AppPermission.All))
                    {
                        Emailer.Send(x.Email, model.Code + " " + model.Title + " role deleted by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Role Deleted");
                    }
                }
                if (AppSettings.GetVal<bool>("notification:Notify"))
                {
                    foreach (var x in appUser.GetByPermission(AppPermission.All))
                    {
                        notify.Create(null, x.Id, model.Id, AppNotificationType.Alert, "Role deleted", AppSettings.GetVal("notification:URL") + "/Secure/AccessControl/Roles", model.Code + " " + model.Title + " role deleted by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    }
                }
                #endregion

                appRole.Delete(Id);

                realtime.UpdateAppUserRoles("Role has been deleted.");

                TempData["SuccessMsg"] = "Role has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/DeleteRole > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost, AppAuthorize(AppPermission.All)]
        public JsonResult DeleteMultipleRoles(string Ids)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        var model = appRole.GetById(new Guid(x));

                        #region Activity Log
                        appLog.Create(null, model.Id, CurrentUser.Id, AppLogType.Activity, "Access Control", model.Code + " " + model.Title + " Role Deleted", "~/Secure/AccessControl/DeleteMultipleRoles > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.Title + "</strong> role deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                        #endregion

                        #region Notifications
                        if (AppSettings.GetVal<bool>("notification:Email"))
                        {
                            foreach (var xx in appUser.GetByPermission(AppPermission.All))
                            {
                                Emailer.Send(xx.Email, model.Code + " " + model.Title + " role deleted by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Role Deleted");
                            }
                        }
                        if (AppSettings.GetVal<bool>("notification:Notify"))
                        {
                            foreach (var xx in appUser.GetByPermission(AppPermission.All))
                            {
                                notify.Create(null, xx.Id, model.Id, AppNotificationType.Alert, "Role deleted", AppSettings.GetVal("notification:URL") + "/Secure/AccessControl/Roles", model.Code + " " + model.Title + " role deleted by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                            }
                        }
                        #endregion

                        appRole.Delete(model.Id);
                    }
                }

                realtime.UpdateAppUserRoles("Multiple roles has been deleted.");

                TempData["SuccessMsg"] = "Selected roles has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/DeleteMultipleRoles > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Remote Functions

        [HttpPost, AppAuthorize(AppPermission.All, AppPermission.Contact)]
        public JsonResult ChangeStatus(Guid Id)
        {
            try
            {
                var model = appUser.GetUserById(Id);
                appUser.ChangeStatus(Id);

                #region Activity Log
                appLog.Create(model.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Access Control", model.Code + " " + model.FullName + " Contact Status Changed", "~/Secure/AccessControl/ChangeStatus > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.FullName + "</strong> contact status changed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateAppUsers("The status of contact has been updated.");

                TempData["SuccessMsg"] = "The status of contact has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/ChangeStatus > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost, AppAuthorize(AppPermission.All, AppPermission.Contact)]
        public JsonResult Delete(Guid Id)
        {
            try
            {
                var model = appUser.GetUserById(Id);

                var dpPath = Server.MapPath("~/Content/Uploads/Dp/");
                if (System.IO.File.Exists(dpPath + Id + ".jpg"))
                {
                    System.IO.File.Delete(dpPath + Id + ".jpg");
                }

                #region Activity Log
                appLog.Create(model.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Access Control", model.Code + " " + model.FullName + " Contact Deleted", "~/Secure/AccessControl/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.FullName + "</strong> contact deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                appUser.Delete(Id);

                realtime.UpdateAppUsers("Contact has been deleted.");
                TempData["SuccessMsg"] = "Contact has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        
        [HttpPost, AppAuthorize(AppPermission.All, AppPermission.Contact)]
        public JsonResult DeleteMultiple(string Ids)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        var model = appUser.GetUserById(new Guid(x));

                        var dpPath = Server.MapPath("~/Content/Uploads/Dp/");
                        if (System.IO.File.Exists(dpPath + model.Id + ".jpg"))
                        {
                            System.IO.File.Delete(dpPath + model.Id + ".jpg");
                        }

                        #region Activity Log
                        appLog.Create(model.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Access Control", model.Code + " " + model.FullName + " Contact Deleted", "~/Secure/AccessControl/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.FullName + "</strong> contact deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                        #endregion

                        appUser.Delete(model.Id);
                    }
                }

                realtime.UpdateAppUsers("Multiple contacts has been deleted.");

                TempData["SuccessMsg"] = "Selected contacts has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Json Requests
        public JsonResult GetAllUsers(Guid? Id)
        {
            var model = new List<AppUser>();
            if (Id.HasValue)
            {
                model = appUser.GetAllByOfficeId(Id.Value);
                foreach (var x in model)
                {
                    x.Role = appRole.GetById(x.RoleId);
                    if (x.OfficeId.HasValue)
                    {
                        x.Office = ofcRepo.GetById(x.OfficeId.Value);
                    }
                }
            }
            else
            {
                model = appUser.GetAll().Where(x => x.Id != CurrentUser.Id).ToList();
                foreach (var x in model)
                {
                    x.Role = appRole.GetById(x.RoleId);
                    if (x.OfficeId.HasValue)
                    {
                        x.Office = ofcRepo.GetById(x.OfficeId.Value);
                    }
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllRoles()
        {
            var model = appRole.GetAll();
            foreach (var x in model)
            {
                x.Contacts = appUser.GetAllByRoleId(x.Id);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRolePermissions(Guid Id)
        {
            //var result = new List<ShippingPackageModel>();
            //if (Id > 0)
            //{
            //    result = shippers.GetCompanyPackages(Id).ToList();
            //}
            //else
            //    result = shippers.GetAllPackages().ToList();

            var result = new AppRole();
            result = appRole.GetById(Id);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}