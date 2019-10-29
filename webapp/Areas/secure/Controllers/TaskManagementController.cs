using BaseApp.Entity;
using BaseApp.System;
using DMS.Logic;
using DMS.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using BaseApp.Logic;
using WebApp.Hubs;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize(AppPermission.All)]
    public class TaskManagementController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();
        private IAppLog appLog;
        private IAppUser appUserRepo;
        private IAppNotification notify;

        public TaskManagementController()
        {
            appLog = db.As<IAppLog>();
            appUserRepo = db.As<IAppUser>();
            notify = db.As<IAppNotification>();
        }

        // GET: Secure/TaskManagement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tasks()
        {
            return View();
        }

        public PartialViewResult _TasksList()
        {
            return PartialView();
        }

        public ActionResult TaskDetails(int id)
        {
            return View();
        }

        public PartialViewResult _UpdateTask()
        {
            return PartialView();
        }

        public ActionResult CreateTask()
        {
            return View();
        }

        public ActionResult _Checklist()
        {
            return PartialView();
        }

        public ActionResult _UploadFiles(Guid id)
        {
            try
            {
                var dmsRepo = db.As<DmsRepository>();
                return PartialView(dmsRepo.SelectById(id));
            }
            catch
            {
                return Redirect(BaseUrl.GetBaseUrl());
            }
            //return PartialView();
        }

        public PartialViewResult _EmployeesList()
        {
            return PartialView();
        }

        public PartialViewResult _TimeManagement()
        {
            return PartialView();
        }

        public PartialViewResult _SuggestedList()
        {
            return PartialView();
        }

        public ActionResult _BrowsePanel()
        {
            return PartialView();
        }

        public ActionResult _LocationTracking()
        {
            return PartialView();
        }

        #region TaskManager
        public ActionResult Settings()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Settings(FormCollection form, string returnUrl)
        {
            try
            {
                var settings = form.AllKeys.Where(x => x != "returnUrl").ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                //AppModule.Update("TaskManager", taskmanagerStatus, taskManagerMessage);

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
    }
}