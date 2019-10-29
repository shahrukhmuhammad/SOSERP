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
using CRM.Entity;
using CRM.Logic;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize(AppPermission.All)]
    public class SettingController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();
        
        private IAppUser appUserRepo;
        private IAppLog appLog;
        private IInsurance insuranceRepo;
        private IAppNotification notify;
        private ITaxTypes taxTypeRepo;
        private IBenefitType btRepo;
        private IBonusType bonusTypeRepo;
        private CertificateSettingsRepository certificateSettingsRepo;
        CertificationsRepository certificatesRepo;
        private IContact crmRepo;
        //IContactRepository contactsRepo;

        public SettingController()
        {
            appUserRepo = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            insuranceRepo = db.As<IInsurance>();
            notify = db.As<IAppNotification>();
            taxTypeRepo = db.As<ITaxTypes>();
            btRepo = db.As<IBenefitType>();
            bonusTypeRepo = db.As<IBonusType>();
            certificateSettingsRepo = db.As<CertificateSettingsRepository>();
            certificatesRepo = db.As<CertificationsRepository>();
            crmRepo = db.As<IContact>();
        }

        public ActionResult Index()
        {
            try
            {
                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Settings", "All Settings Viewed", "~/Secure/Setting/Index", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>All settings viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                return View();
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Settings", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/Index", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";

                return RedirectToAction(Url.Action("Error", "AppLog", new { Area = "Secure" }));
            }
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Index(FormCollection form, string returnUrl, HttpPostedFileBase logo, HttpPostedFileBase favicon)
        {
            try
            {
                var settings = form.AllKeys.ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Settings", "Settings Updated", "~/Secure/Setting/Index > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Settings updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion
                TempData["SuccessMsg"] = "Settings have been saved successfully.";

                var path = Server.MapPath("~/Content/Images/");
                if (logo.HasValue())
                {
                    logo.SaveAs(path + "applogo.png");
                }
                if (favicon.HasValue())
                {
                    favicon.SaveAs(Server.MapPath("~/")+"appfavicon.ico");
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Settings", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/Index > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
            //return Content("<script> location.reload(true); </script>");
        }

        #region Application Settings

        #region CMS Settings
        public ActionResult CMS()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult CMS(FormCollection form, HttpPostedFileBase logo, HttpPostedFileBase favicon, bool cmsStatus, string cmsMessage, string returnUrl)
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

                AppModule.Update("CMS", cmsStatus, cmsMessage);
                

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Content Management Module Updated", "~/Secure/Setting/CMS > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Content management module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
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
                        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Content Management Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/CMS", "Content Management module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    }
                }

                realtime.UpdateNotifications("Content Management module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "Content Management has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Setting", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/CMS > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
        }
        #endregion

        #region ECommerce Settings
        public ActionResult Ecommerce()
        {
            //var model = shippers.GetAll();
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Ecommerce(FormCollection form, bool ecommerceStatus, string ecommerceMessage, string returnUrl)
        {

            try
            {
                var settings = form.AllKeys.Where(x => x != "ecommerceMessage" && x != "ecommerceStatus" && x != "returnUrl" && x != "shippingCompany-dataTable_length" && x != "shippingPackages-dataTable_length" && x != "shippingPostCodes-dataTable_length").ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                AppModule.Update("Ecommerce", ecommerceStatus, ecommerceMessage);


                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Ecommerce Module Updated", "~/Secure/Setting/Ecommerce > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Ecommerce module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                //if (AppSettings.GetVal<bool>("notification:Email"))
                //{
                //    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                //    {
                //        Emailer.Send(x.Email, "Content Management module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Content Management Module Updated");
                //    }
                //}
                //if (AppSettings.GetVal<bool>("notification:Notify"))
                //{
                //    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                //    {
                //        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Content Management Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/CMS", "Content Management module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                //    }
                //}

                realtime.UpdateNotifications("Ecommerce module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "Ecommerce has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Setting", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/CMS > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
        }

        #endregion

        #region DMS
        public ActionResult DMS()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DMS(bool dmsStatus, string dmsMessage)
        {
            try
            {
                AppModule.Update("DMS", dmsStatus, dmsMessage);

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

        #region Employees
        public ActionResult Employees()
        {
            return View();
        }

        [HttpPost, ValidateInput(false), ValidateAntiForgeryToken]
        public ActionResult Employees(FormCollection form, bool employeeStatus, string employeeMessage, string returnUrl)
        {
            try
            {
                var settings = form.AllKeys.Where(x => x != "employeeMessage" && x != "employeeStatus" && x != "returnUrl").ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                AppModule.Update("Employees", employeeStatus, employeeMessage);


                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Employees Management Module Updated", "~/Secure/Setting/Employees > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Employees management module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                if (AppSettings.GetVal<bool>("notification:Email"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        Emailer.Send(x.Email, "Employees Management module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Employees Management Module Updated");
                    }
                }
                if (AppSettings.GetVal<bool>("notification:Notify"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Employees Management Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/Employees", "Employees Management module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    }
                }

                realtime.UpdateNotifications("Employees Management module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "Employees Management has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Setting", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/Employees > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
        }
        #endregion

        #region CRM
        public ActionResult CRM()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult CRM(FormCollection form, bool crmStatus, string crmMessage, string returnUrl)
        {
            try
            {
                var settings = form.AllKeys.Where(x => x != "crmMessage" && x != "crmStatus" && x != "returnUrl").ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                AppModule.Update("CRM", crmStatus, crmMessage);


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
        #endregion

        #region TaskManager
        public ActionResult TaskManager()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult TaskManager(FormCollection form, string returnUrl, bool taskmanagerStatus, string taskManagerMessage)
        {
            try
            {
                var settings = form.AllKeys.Where(x => x != "taskManagerMessage" && x != "taskmanagerStatus" && x != "returnUrl").ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                AppModule.Update("TaskManager", taskmanagerStatus, taskManagerMessage);


                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Task Manager Module Updated", "~/Secure/Setting/TaskManager > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Task Manager module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                #region Notifications
                if (AppSettings.GetVal<bool>("notification:Email"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        Emailer.Send(x.Email, "Task Manager module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Task Manager Module Updated");
                    }
                }
                if (AppSettings.GetVal<bool>("notification:Notify"))
                {
                    foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                    {
                        notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Task Manager Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/TaskManager", "Task Manager module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    }
                }

                realtime.UpdateNotifications("Task Manager module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                #endregion

                TempData["SuccessMsg"] = "Task Manager has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Setting", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/TaskManager > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
        }
        #endregion

        #region Financials
        public ActionResult Financials()
        {
            return View();
        }
        #endregion

        #endregion

        #region Notification Settings
        
        #endregion

        #region Templates

        #region Email Template
        public ActionResult EmailTemplate(string id)
        {
            var repo = db.As<IEmailTemplateRepository>();
            var template = new EmailTemplate();
            if (!string.IsNullOrEmpty(id))
            {
                template = repo.GetById(id);
                if (Request.IsAjaxRequest())
                {
                    return Json(template, JsonRequestBehavior.AllowGet);
                }
            }
            return View(!string.IsNullOrEmpty(id) ? template : null);
        }
        [HttpPost, ValidateInput(false), ValidateAntiForgeryToken]
        public ActionResult EmailTemplate(EmailTemplate template)
        {
            try
            {
                var repo = db.As<IEmailTemplateRepository>();
                repo.Save(template);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", template.Id.ToSpacedTitleCase() + " Email Template Updated", "~/Secure/Setting/EmailTemplate > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + template.Id.ToSpacedTitleCase() + "</strong> email template updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Template has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Setting", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/EmailTemplate > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return View(template.Id.HasValue() ? template : null);
        }
        #endregion

        #region Form Submission
        public ActionResult FormSubmission()
        {
            return View();
        }
        #endregion

        #endregion

        public ActionResult System()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveEmployeeTaskCreationSettings(bool TaskCreation = false)
        {
            AppSettings.SetVal("employee:TaskCreation", TaskCreation.ToString().ToLowerInvariant());
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveEmployeeAcceptNRejectTask(bool AcceptNReject = false)
        {
            AppSettings.SetVal("employee:AcceptNRejectTask", AcceptNReject.ToString().ToLowerInvariant());
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveEmployeeInsurance(bool EmployeeInsurance = false)
        {
            AppSettings.SetVal("employee:Insurance", EmployeeInsurance.ToString().ToLowerInvariant());
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddEmployeeSettings(List<Insurance> Insurance, List<TaxTypes> tTypes, List<BenefitType> bTypes, List<BonusType> bonusTypes, List<CertificateSettings> certificateTypes)
        {
            try
            {
                #region Insurance
                if (Insurance != null)
                {
                    foreach (var item in Insurance)
                    {
                        item.CreatedById = CurrentUser.Id;
                        item.CreatedOn = DateTime.UtcNow;
                        item.UpdatedOn = DateTime.UtcNow;
                        insuranceRepo.Insert(item);
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Employee Insurance Module Updated", "~/Secure/Setting/Employees > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Employee Insurance module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region Notifications
                    if (AppSettings.GetVal<bool>("notification:Email"))
                    {
                        foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                        {
                            Emailer.Send(x.Email, "Employee Insurance module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Employee Insurance Module Updated");
                        }
                    }
                    if (AppSettings.GetVal<bool>("notification:Notify"))
                    {
                        foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                        {
                            notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Employee Insurance Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/Employees", "Employee Insurance module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }
                    }

                    realtime.UpdateNotifications("Employee Insurance module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    #endregion

                    TempData["SuccessMsg"] = "Employee Insurance has been updated successfully.";
                }
                #endregion

                #region TaxTypes
                else if (tTypes != null)
                {
                    foreach (var item in tTypes)
                    {
                        item.CreatedById = CurrentUser.Id;
                        item.CreatedOn = DateTime.UtcNow;
                        item.UpdatedOn = DateTime.UtcNow;
                        taxTypeRepo.Insert(item);
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Tax Types Module Updated", "~/Secure/Setting/Employees > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Tax Types module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region Notifications
                    if (AppSettings.GetVal<bool>("notification:Email"))
                    {
                        foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                        {
                            Emailer.Send(x.Email, "Tax Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Employee Insurance Module Updated");
                        }
                    }
                    if (AppSettings.GetVal<bool>("notification:Notify"))
                    {
                        foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                        {
                            notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Tax Types Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/Employees", "Tax Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }
                    }

                    realtime.UpdateNotifications("Tax Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    #endregion

                    TempData["SuccessMsg"] = "Tax Types has been updated successfully.";
                }
                #endregion

                #region BenefitTypes
                
                else if (bTypes != null)
                {
                    foreach (var item in bTypes)
                    {
                        item.CreatedById = CurrentUser.Id;
                        item.CreatedOn = DateTime.UtcNow;
                        item.UpdatedOn = DateTime.UtcNow;
                        btRepo.Insert(item);
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Benefit Types Module Updated", "~/Secure/Setting/Employees > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Benefit Types module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region Notifications
                    if (AppSettings.GetVal<bool>("notification:Email"))
                    {
                        foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                        {
                            Emailer.Send(x.Email, "Benefit Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Benefit Types Module Updated");
                        }
                    }
                    if (AppSettings.GetVal<bool>("notification:Notify"))
                    {
                        foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                        {
                            notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Benefit Types Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/Employees", "Benefit Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }
                    }

                    realtime.UpdateNotifications("Benefit Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    #endregion

                    TempData["SuccessMsg"] = "Benefit Types has been updated successfully.";
                }

                #endregion

                #region BonusTypes
                
                else if (bonusTypes != null)
                {
                    foreach (var item in bonusTypes)
                    {
                        item.CreatedById = CurrentUser.Id;
                        item.CreatedOn = DateTime.UtcNow;
                        item.UpdatedOn = DateTime.UtcNow;
                        bonusTypeRepo.Insert(item);
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Bonus Types Module Updated", "~/Secure/Setting/Employees > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Bonus Types module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region Notifications
                    if (AppSettings.GetVal<bool>("notification:Email"))
                    {
                        foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                        {
                            Emailer.Send(x.Email, "Bonus Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Bonus Types Module Updated");
                        }
                    }
                    if (AppSettings.GetVal<bool>("notification:Notify"))
                    {
                        foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                        {
                            notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Bonus Types Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/Employees", "Bonus Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }
                    }

                    realtime.UpdateNotifications("Bonus Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    #endregion

                    TempData["SuccessMsg"] = "Bonus Types has been updated successfully.";
                }

                #endregion

                #region CertificateSetting

                else if (certificateTypes != null)
                {
                    var sep = new string[] { "," };
                    foreach (var item in certificateTypes)
                    {
                        item.Id = certificateSettingsRepo.Insert(item);

                        foreach (var c in item.AppliesTo.Split(sep, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var cType = EnumHelperEx.ParseEnum<AppUserEmploymentStatus>(c);
                            var users = appUserRepo.SelectAllByType(cType);
                            foreach (var u in users)
                            {
                                certificatesRepo.DeleteByUserIdnCertificateId(u.Id, item.Id);
                                var certificate = new Certifications
                                {
                                    CertificateId = item.Id,
                                    UserId = u.Id,
                                    Status = CertificateStatus.Pending,
                                    ExpiryDate = DateTime.UtcNow.AddYears(1)
                                };
                                certificatesRepo.Insert(certificate);
                            }
                        }
                    }

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Setting", "Certificate Types Module Updated", "~/Secure/Setting/Employees > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Certificate Types module updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region Notifications
                    if (AppSettings.GetVal<bool>("notification:Email"))
                    {
                        foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                        {
                            Emailer.Send(x.Email, "Certificate Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "Certificate Types Module Updated");
                        }
                    }
                    if (AppSettings.GetVal<bool>("notification:Notify"))
                    {
                        foreach (var x in appUserRepo.GetByPermission(AppPermission.All))
                        {
                            notify.Create(x.OfficeId, x.Id, null, AppNotificationType.Alert, "Certificate Types Module Updated", AppSettings.GetVal("notification:URL") + "/Secure/Setting/Employees", "Certificate Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }
                    }

                    realtime.UpdateNotifications("Certificate Types module updated by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                    #endregion

                    TempData["SuccessMsg"] = "Certificate Types has been updated successfully.";
                }

                #endregion
            }
            catch (Exception e)
            {
                var Message = e.Message;
                throw;
            }
            return Redirect("~/Secure/Setting/Employees");
        }

        public ActionResult SaveDeductTaxTypes(bool DeductTaxes = false)
        {
            AppSettings.SetVal("employee:DeductTaxes", DeductTaxes.ToString().ToLowerInvariant());
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _TaxTypes()
        {
            var model = taxTypeRepo.SelectAll();
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult DeleteTaxTypesRecord(Guid Id)
        {
            taxTypeRepo.Delete(Id);
            return Json(true);
        }

        public ActionResult SaveBenefitsType(bool BenefitsType = false)
        {
            AppSettings.SetVal("employee:BenefitsType", BenefitsType.ToString().ToLowerInvariant());
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _BenefitsType()
        {
            var model = btRepo.SelectAll();
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult DeleteBenefitTypeRecord(Guid Id)
        {
            btRepo.Delete(Id);
            return Json(true);
        }

        public ActionResult SaveBonusType(bool BonusType = false)
        {
            AppSettings.SetVal("employee:Bonus", BonusType.ToString().ToLowerInvariant());
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _EmployeeBonus()
        {
            var model = bonusTypeRepo.SelectAll();
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult DeleteEmployeeBonusRecord(Guid Id)
        {
            bonusTypeRepo.Delete(Id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveEmployeeCertificateSetting(bool certificate = false)
        {
            AppSettings.SetVal("employee:Certificates", certificate.ToString().ToLowerInvariant());
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _Certificates()
        {
            var model = certificateSettingsRepo.SelectList();
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult DeleteEmployeeCertificatesRecord(Guid Id)
        {
            certificateSettingsRepo.Delete(Id);
            certificatesRepo.DeleteByCertificateId(Id);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #region Notifications

        public ActionResult Systems()
        {
            return View();
        }
        #endregion

        #region Save Grid PageLength
        [HttpPost]
        public ActionResult SavePageLength(string length)
        {
            if (length != null)
            {
                appUserRepo.UpdatePageLength(CurrentUser.Id, length);
            }
            return new EmptyResult();
        }
        #endregion
    }
}