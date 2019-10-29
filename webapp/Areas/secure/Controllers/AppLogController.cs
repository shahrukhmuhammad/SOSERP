using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Web.Mvc;
using Insight.Database;
using BaseApp.Entity;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize]
    public class AppLogController : AppController
    {
        private IOffice ofcRepo;
        private IAppLog appLog;
        private IAppUser appUser;

        public AppLogController()
        {
            ofcRepo = db.As<IOffice>();
            appLog = db.As<IAppLog>();
            appUser = db.As<IAppUser>();

            ViewBag.AllOffices = ofcRepo.GetAll();
        }

        public ActionResult Index()
        {
            var model = appLog.GetByTypeAndContactId(AppLogType.Activity, CurrentUser.Id);
            foreach (var x in model)
            {
                if (x.OfficeId.HasValue)
                {
                    x.Office = ofcRepo.GetById(x.OfficeId.Value);
                }
                if (x.ContactId.HasValue)
                {
                    x.Contact = appUser.GetUserById(x.ContactId.Value);
                }
            }
            return View(model);
        }

        [AppAuthorize(AppPermission.All, AppPermission.LogsManagement)]
        public ActionResult General(Guid? Id)
        {
            if (Id.HasValue)
            {
                #region Actions
                var office = ofcRepo.GetById(Id.Value);
                office.Contact = appUser.GetUserById(office.ContactId);
                ViewBag.Office = office;

                var model = appLog.GetByTypeAndOfficeId(AppLogType.Activity, Id.Value);
                foreach (var x in model)
                {
                    if (x.OfficeId.HasValue)
                    {
                        x.Office = ofcRepo.GetById(x.OfficeId.Value);
                    }
                    if (x.ContactId.HasValue)
                    {
                        x.Contact = appUser.GetUserById(x.ContactId.Value);
                    }
                }
                #endregion

                return View(model);
            }
            else
            {
                #region Actions
                var model = appLog.GetByType(AppLogType.Activity);
                foreach (var x in model)
                {
                    if (x.OfficeId.HasValue)
                    {
                        x.Office = ofcRepo.GetById(x.OfficeId.Value);
                    }
                    if (x.ContactId.HasValue)
                    {
                        x.Contact = appUser.GetUserById(x.ContactId.Value);
                    }
                }
                #endregion

                return View(model);
            }
        }

        [AppAuthorize(AppPermission.All, AppPermission.LogsManagement)]
        public ActionResult Error(Guid? Id)
        {
            if (Id.HasValue)
            {
                #region Actions
                var office = ofcRepo.GetById(Id.Value);
                office.Contact = appUser.GetUserById(office.ContactId);
                ViewBag.Office = office;

                var model = appLog.GetByTypeAndOfficeId(AppLogType.Error, Id.Value);
                foreach (var x in model)
                {
                    if (x.OfficeId.HasValue)
                    {
                        x.Office = ofcRepo.GetById(x.OfficeId.Value);
                    }
                    if (x.ContactId.HasValue)
                    {
                        x.Contact = appUser.GetUserById(x.ContactId.Value);
                    }
                }
                #endregion

                return View(model);
            }
            else
            {
                #region Actions
                var model = appLog.GetByType(AppLogType.Error);
                foreach (var x in model)
                {
                    if (x.OfficeId.HasValue)
                    {
                        x.Office = ofcRepo.GetById(x.OfficeId.Value);
                    }
                    if (x.ContactId.HasValue)
                    {
                        x.Contact = appUser.GetUserById(x.ContactId.Value);
                    }
                }
                #endregion

                return View(model);
            }
        }

        public ActionResult Details(Guid Id)
        {
            #region Actions
            var model = appLog.GetById(Id);
            if (model.ContactId.HasValue)
            {
                model.Contact = appUser.GetUserById(model.ContactId.Value);
            }
            if (model.OfficeId.HasValue)
            {
                model.Office = ofcRepo.GetById(model.OfficeId.Value);
            }
            #endregion

            return View(model);
        }

        [AppAuthorize(AppPermission.All, AppPermission.LogsManagement), HttpPost]
        public JsonResult SendReport(Guid Id)
        {
            var errorLog = appLog.GetById(Id);
            if (errorLog.OfficeId.HasValue)
            {
                errorLog.Office = ofcRepo.GetById(errorLog.OfficeId.Value);
            }
            if (errorLog.ContactId.HasValue)
            {
                errorLog.Contact = appUser.GetUserById(errorLog.ContactId.Value);
            }

            var msg = "";

            try
            {
                #region Email
                var msgBody = "<table>";
                msgBody += "<tr><th>Type</th><td>" + errorLog.Type.ToSpacedTitleCase() + "</td></tr>";
                msgBody += "<tr><th>Office</th><td>" + (errorLog.OfficeId.HasValue ? errorLog.Office.Title : "N / A") + "</td></tr>";
                msgBody += "<tr><th>Contact</th><td>" + (errorLog.ContactId.HasValue ? errorLog.Contact.FullName : "N / A") + "</td></tr>";
                msgBody += "<tr><th>Title</th><td>" + errorLog.Title + "</td></tr>";
                msgBody += "<tr><th>Module</th><td>" + errorLog.Module + "</td></tr>";
                msgBody += "<tr><th>Location</th><td>" + errorLog.Location + "</td></tr>";
                msgBody += "<tr><th>Created On</th><td>" + errorLog.CreatedOn.ToString("dd/MM/yyyy hh:mm tt") + "</td></tr>";
                msgBody += "</table>";
                msgBody += errorLog.Description;

                Emailer.Send(AppSettings.GetVal("support:Email"), msgBody, "Error Log Report From " + AppSettings.GetVal("app:Title") + " Version:" + AppSettings.GetVal("app:Version"));
                #endregion

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Application Log", "Sent error report to support", "~/Secure/AppLog/SendReport > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Email sent to the email <strong>" + AppSettings.GetVal("support:Email") + "</strong> of support centre.</td></tr></table>");
                #endregion

                msg = "Email sent successfully to the support centre.";
                TempData["SuccessMsg"] = "Email sent successfully to the support centre.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Application Log", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AppLog/SendReport > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                msg = "We have encountered an error while processing your request, Please see log for details.";
                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(msg);
        }

        [HttpPost]
        public JsonResult DeleteMultiple(string Ids)
        {
            var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!string.IsNullOrEmpty(Ids))
            {
                foreach (var x in idsList)
                {
                    appLog.Delete(new Guid(x));
                }
            }
            return Json(true);
        }
    }
}