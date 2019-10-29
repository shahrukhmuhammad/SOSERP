using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using System.Net.Mail;
using System.Net;
using WebApp.Hubs;
using BaseApp;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize(AppPermission.All)]
    public class ConfigController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IOffice officeRepo;
        private IAppUser appUserRepo;
        private IAppLog appLog;
        private IAppNotification notify;
        private IAppModule appModule;
        private IAppSMTP appSmtp;

        public ConfigController()
        {
            officeRepo = db.As<IOffice>();
            appUserRepo = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            notify = db.As<IAppNotification>();
            appModule = db.As<IAppModule>();
            appSmtp = db.As<IAppSMTP>();
        }

        #region Index
        public ActionResult Index()
        {
            try
            {
                var allOffices = officeRepo.GetAll();
                foreach (var x in allOffices)
                {
                    x.Contact = appUserRepo.GetUserById(x.ContactId);
                }
                ViewBag.AllOffices = allOffices;

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Configuration", "All Configurations Viewed", "~/Secure/Config/Index", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>All configurations viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                return View();
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Configuration", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Config/Index", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion
                
                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";

                return RedirectToAction(Url.Action("Error", "AppLog", new { Area = "Secure" }));
            }
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Index(FormCollection form, string returnUrl)
        {
            try
            {
                var settings = form.AllKeys.ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Configuration", "Configurations Updated", "~/Secure/Config/Index > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Configurations updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                if (AppSettings.GetVal<bool>("notification:Email"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        Emailer.Send(x.Email, "Configurations updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Configurations Updated");
                    }
                }
                if (AppSettings.GetVal<bool>("notification:Notify"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Configurations Updated", Request.Url.ToString(), "Configurations updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    }
                }

                realtime.UpdateNotifications("Configurations updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "Configurations have been saved successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Configuration", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Config/Index > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
        }
        #endregion

        #region SMTP Controls
        public ActionResult Smtps()
        {
            var mode = appSmtp.GetAll();
            ViewBag.Modules = appSmtp.GetModules();
            return View(mode);
        }

        public ActionResult Smtp(Guid? Id)
        {
            var model = new AppSMTP();
            ViewBag.ModulesList = appModule.GetAll();
            if (Id.HasValue)
            {
                model = appSmtp.GetById(Id.Value);
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Smtp(AppSMTP model)
        {
            try
            {
                if (model.Id.IsEmpty())
                {
                    model.Id = appSmtp.Create(model);
                    if (model.IsDefault)
                    {
                        appSmtp.UpdateStatus(model.Id);
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Configuration", "Smtp Control Added", "~/Secure/Config/Smtp > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Smtp control of <strong>" + model.Title + "</strong> added by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateSmtpControls(model.Title + " smtp control have been added.");

                    TempData["SuccessMsg"] = model.Title + " smtp control have been added successfully.";
                }
                else
                {
                    appSmtp.Update(model);
                    if (model.IsDefault)
                    {
                        appSmtp.UpdateStatus(model.Id);
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Configuration", "Smtp Control Updated", "~/Secure/Config/Smtp > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Smtp control for <strong>" + model.Title + "</strong> updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateSmtpControls(model.Title + " smtp control have been updated.");

                    TempData["SuccessMsg"] = model.Title + " smtp control have been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Configuration", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Config/Smtp > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Smtps");
        }

        [HttpPost]
        public JsonResult SmtpTest(FormCollection form)
        {
            var msg = "Email sent successfully to ";
            try
            {
                #region Actions
                using (var client = new SmtpClient
                {
                    Host = form["OutgoingServer"],
                    Port = Convert.ToInt32(form["OutgoingPort"]),
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(form["DefaultUsername"], form["DefaultPassword"]),
                    EnableSsl = Convert.ToBoolean(form["SSL"]),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                })
                {
                    var emailMessage = new MailMessage
                    {
                        From = new MailAddress(form["DefaultUsername"], AppSettings.GetVal("app:Title"))
                    };
                    emailMessage.To.Add(form["DefaultUsername"]);

                    emailMessage.IsBodyHtml = true;
                    emailMessage.Body = "SMTP settings work correctly. This is a testing email.";
                    emailMessage.Subject = "SMTP Settings Test";
                    client.Send(emailMessage);
                    msg += form["DefaultUsername"];
                }
                #endregion

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Configuration", "Application Smtp Test Email", "~/Secure/Config/SmtpTest > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Sent application SMTP settings test email by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Email sent successfully to " + form["DefaultUsername"] + ".";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += ex.InnerException.Message;
                }
                TempData["ErrorMsg"] = msg;

                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Configuration", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Config/SmtpTest > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion
            }
            return Json(msg);
        }

        [HttpPost]
        public JsonResult CheckDuplicateSmtpTitle(Guid Id, string Title)
        {
            if (appSmtp.UniqueTitle(Id, Title))
            {
                return Json(true);
            }
            else
            {
                return Json("Title already exists.");
            }
        }

        [HttpPost]
        public JsonResult DeleteSmtp(Guid Id)
        {
            try
            {
                appSmtp.Delete(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, Id, CurrentUser.Id, AppLogType.Activity, "Configurations", "Smtp Control deleted", "~/Secure/Config/DeleteSmtp > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Smtp control deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateSmtpControls("Smtp control has been deleted.");

                TempData["SuccessMsg"] = "Smtp control has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Configurations", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Config/DeleteSmtp > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteMultipleSmtps(string Ids)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        appSmtp.Delete(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Configurations", "Multiple smtp controls deleted", "~/Secure/Config/DeleteMultipleSmtps > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple smtp controls deleted deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateSmtpControls("Multiple smtp controls has been deleted.");

                TempData["SuccessMsg"] = "Selected smtp controls deleted.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Configurations", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Config/DeleteMultipleSmtps > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Information
        public ActionResult Info()
        {
            try
            {
                #region Actions
                const string SqlVersionInfo = "SELECT @@VERSION VersionInfo;";
                ViewBag.SqlVersionInfo = db.SingleSql<string>(SqlVersionInfo).Replace("\n", "<br>");
                ViewBag.ConnectionString = db.ConnectionString;
                AspNetHostingPermissionLevel hostingTrustLevel = AspNetHostingPermissionLevel.None;

                foreach (var trustLevel in
                    new[]
                        {
                        AspNetHostingPermissionLevel.Unrestricted,
                        AspNetHostingPermissionLevel.High,
                        AspNetHostingPermissionLevel.Medium,
                        AspNetHostingPermissionLevel.Low,
                        AspNetHostingPermissionLevel.Minimal
                        })
                {
                    try
                    {
                        new AspNetHostingPermission(trustLevel).Demand();
                        hostingTrustLevel = trustLevel;
                        break;
                    }
                    catch (System.Security.SecurityException)
                    {
                        continue;
                    }


                }
                ViewBag.TrustLevel = hostingTrustLevel;
                var crInstalled = "";
                try
                {
                    AppDomain dom = AppDomain.CreateDomain("cr");
                    //13.0.2000.0 is Crystal Reports for VS
                    var assembly = dom.Load("CrystalDecisions.CrystalReports.Engine, Version=13.0.2000.0, Culture=neutral,PublicKeyToken=692fbea5521e1304");
                    var a = assembly.GetTypes();
                    AppDomain.Unload(dom);
                    foreach (var t in a)
                    {
                        crInstalled = t.Assembly.GetName().Version.ToString();
                    }
                }
                catch (Exception ex)
                {
                    crInstalled = "Not Installed";
                }
                ViewBag.CrystalReports = crInstalled;

                #endregion

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Configuration", "Application Server Informations Viewed", "~/Secure/Config/Info", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Applications's server information viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                return View();
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Configuration", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Config/Info", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";

                return RedirectToAction(Url.Action("Error", "AppLog", new { Area = "Secure" }));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RestartApp(string returnUrl)
        {
            try
            {
                HttpRuntime.UnloadAppDomain();

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Configuration", "Application Restarted", "~/Secure/Config/RestartApp > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Application restarted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                if (AppSettings.GetVal<bool>("notification:Email"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        Emailer.Send(x.Email, "Application restarted by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Application Restarted");
                    }
                }
                if (AppSettings.GetVal<bool>("notification:Notify"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Application Restarted", AppSettings.GetVal("notification:URL") + "/Secure/Config/Info", "Application restarted by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    }
                }

                realtime.UpdateNotifications("Application restarted by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "Application was restarted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Configuration", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Config/RestartApp > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return string.IsNullOrEmpty(returnUrl) ? RedirectToAction("Index", "Dashboard", new { Area = "Dashboard" }) : RedirectToLocal(returnUrl);
        }
        #endregion

        #region Json Requests

        public JsonResult GetAllSmtpControls()
        {
            var model = appSmtp.GetAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult _Configuration(int? Id)
        {
            var model = new AppSMTP();
            if (Id > 0)
            {
                model = appSmtp.GetByThirdParty(Id.Value);
            }
            return PartialView(model);
        }

        public ActionResult _ConfigureModule(Guid Id)
        {
            var model = new AppModuleSMTP();
            model.SMTPId = Id;
            ViewBag.ModulesList = appModule.GetAll();
            ViewBag.SMTPModules = appSmtp.GetModules();
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult _ConfigureModule(AppSMTP model)
        {
            return RedirectToAction("smtps","Config");
        }

        [HttpPost]
        public JsonResult SaveModulesSettings(string Ids, Guid Id)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        var model = new AppModuleSMTP();
                        model.SMTPId = Id;
                        model.ModuleId = x;
                        appSmtp.Create(model);
                    }
                }

                #region Activity Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Configurations", "Multiple smtp controls deleted", "~/Secure/Config/DeleteMultipleSmtps > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple smtp controls deleted deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                //realtime.UpdateSmtpControls("Multiple smtp controls has been deleted.");

                //TempData["SuccessMsg"] = "Selected smtp controls deleted.";
            }
            catch (Exception ex)
            {
                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Configurations", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Config/DeleteMultipleSmtps > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                //TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
    }
}