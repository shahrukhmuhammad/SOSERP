using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using BaseApp.System;
using BaseApp.Logic;
using BaseApp.Entity;
using WebApp.Hubs;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize]
    public class AppNotificationController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppNotification notify;
        private IAppLog appLog;
        private IAppUser appUser;
        private IOffice ofcRepo;

        public AppNotificationController()
        {
            notify = db.As<IAppNotification>();
            appLog = db.As<IAppLog>();
            appUser = db.As<IAppUser>();
            ofcRepo = db.As<IOffice>();
        }

        public ActionResult Index(AppNotificationType? Id)
        {
            var model = new List<AppNotification>();
            try
            {
                if (Id.HasValue)
                {
                    model = notify.GetByContactIdAndType(CurrentUser.Id, AppNotificationType.Warning);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "App Notification", "All alerts viewed", "~/Secure/AppNotification/Index", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>All alerts viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion
                }
                else
                {
                    model = notify.GetByContactIdAndType(CurrentUser.Id, AppNotificationType.Alert);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "App Notification", "All warnings viewed", "~/Secure/AppNotification/Index", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>All alerts viewed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "App Notification", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AppNotification/Index", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return View(model);
        }

        public ActionResult Details(Guid Id)
        {
            var model = notify.GetById(Id);
            model.Contact = appUser.GetUserById(model.ContactId);
            if (model.OfficeId.HasValue)
            {
                model.Office = ofcRepo.GetById(model.OfficeId.Value);
            }
            return View(model);
        }

        public PartialViewResult Reader()
        {
            return PartialView(notify.GetRecentByContactIdAndType(CurrentUser.Id, AppNotificationType.Alert));
        }

        public PartialViewResult WarningReader()
        {
            return PartialView(notify.GetRecentByContactIdAndType(CurrentUser.Id, AppNotificationType.Warning));
        }

        #region Remote Functions
        [HttpPost]
        public JsonResult MarkReadAll()
        {
            try
            {
                notify.ReadAllByContactId(CurrentUser.Id);
                realtime.UpdateNotifications("Marked all alerts as read.");

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "App Notification", "Marked all alerts as read", "~/Secure/AppNotification/MarkAllRead > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Marked all alerts as read by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "App Notification", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AppNotification/MarkAllRead > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult MarkRead(Guid Id)
        {
            try
            {
                notify.MarkRead(Id);
                realtime.UpdateNotifications("Alert marked as read.");

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "App Notification", "Alert mark as read", "~/Secure/AppNotification/MarkRead > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Alert marked as read by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "App Notification", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AppNotification/MarkRead > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteMultiple(string Ids)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        notify.Delete(new Guid(x));
                    }
                }

                realtime.UpdateNotifications("Multiple alerts deleted.");

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "App Notification", "Alert deleted", "~/Secure/AppNotification/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Alert deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "App Notification", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AppNotification/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            
            return Json(true);
        }
        #endregion
    }
}