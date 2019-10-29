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
using Timesheet.Logic;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize(AppPermission.All, AppPermission.ViewContact, AppPermission.Contact)]
    public class EmployeeController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppUser appUser;
        private IAppLog appLog;
        private IOffice ofcRepo;
        private IAppNotification notify;
        private IAppRole appRole;
        private IOffice ofc;
        private IInsurance insuranceRepo;

        private IExtraFieldSection extras;
        private IPtoCode ptoCode;
        private ITimesheet timesheet;

        public EmployeeController()
        {
            appUser = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            ofcRepo = db.As<IOffice>();
            notify = db.As<IAppNotification>();
            appRole = db.As<IAppRole>();
            ofc = db.As<IOffice>();
            insuranceRepo = db.As<IInsurance>();
            extras = db.As<IExtraFieldSection>();
            ptoCode = db.As<IPtoCode>();
            timesheet = db.As<ITimesheet>();

            ViewBag.AllOffices = ofcRepo.GetAll();
        }

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult _AllEmployees()
        {
            var model = appUser.GetAll();
            foreach (var x in model)
            {
                if (!x.RoleId.IsEmpty())
                {
                    x.Role = appRole.GetById(x.RoleId);
                }
                if (x.OfficeId.HasValue)
                {
                    x.Office = ofc.GetById(x.OfficeId.Value);
                }
            }
            return PartialView(model);
        }

        #region Profile View
        public ActionResult Details(Guid Id)
        {
            var extraFieldSection = extras.GetByModule("Employee");
            foreach (var x in extraFieldSection)
            {
                x.Fields = extras.GetByParentId(x.Id);
            }
            ViewBag.Extras = extraFieldSection;

            var model = appUser.GetUserById(Id);
            if (model.OfficeId.HasValue)
            {
                model.Office = ofcRepo.GetById(model.OfficeId.Value);
                if (model.Office != null)
                {
                    model.Office.Contact = appUser.GetUserById(model.Office.ContactId);
                }
            }

            model.Extras = appUser.GetExtrasByAppUserId(Id);
            return View(model);
        }
        #endregion

        #region Employee Record
        public ActionResult Record(Guid? Id)
        {
            ViewBag.AllOffices = ofc.GetBreadCrumb();
            var extraFieldSection = extras.GetByModule("Employee");
            foreach (var x in extraFieldSection)
            {
                x.Fields = extras.GetByParentId(x.Id);
            }
            ViewBag.Extras = extraFieldSection;

            var model = new AppUser();
            if (Id.HasValue)
            {
                model = appUser.GetUserById(Id.Value);
                model.Extras = appUser.GetExtrasByAppUserId(Id.Value);
                model.Employment = appUser.GetEmploymentByAppUserId(Id.Value);
            }
            else
            {
                model.Code = appUser.GetMaxCode();
                model.DateOfBirth = DateTime.UtcNow.AddYears(-10);
                model.Extras = new List<AppUserExtra>();
                model.Employment = new AppUserEmployment();
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Record(AppUser model, HttpPostedFileBase file)
        {
            try
            {
                if (model.Id.IsEmpty())
                {
                    model.Status = AppUserStatus.UnVerified;

                    model.Username = model.Code;
                    model.Password = Uuid.Random(8);
                    model.CreatedByUserId = model.UpdatedByUserId = CurrentUser.Id;
                    model.Id = appUser.Create(model);
                    Emailer.Send(model.Email, EmailTemplateType.UserSignup, model);

                    #region Extra Fields
                    if (model.Extras != null)
                    {
                        foreach (var x in model.Extras)
                        {
                            var newExtra = extras.GetById(x.ExtraId);
                            if (newExtra.FieldType == "checkbox")
                            {
                                if (string.IsNullOrEmpty(x.Value) || x.Value == "no")
                                {
                                    x.Value = "false";
                                }
                            }
                            x.AppUserId = model.Id;
                            appUser.CreateExtraField(x);
                        }
                    }
                    #endregion

                    #region Employment
                    model.Employment.AppUserId = model.Id;
                    model.Employment.Monday = model.Employment.Tuesday = model.Employment.Wednesday = model.Employment.Thursday = model.Employment.Friday = model.Employment.Saturday = model.Employment.Sunday = false;
                    appUser.CreateEmployment(model.Employment);
                    #endregion

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Employee Management", model.Code + " " + model.FullName + " Employee Created", "~/Secure/Employee/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.FullName + "</strong> employee created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region Notifications
                    if (AppSettings.GetVal<bool>("notification:Email"))
                    {
                        foreach (var x in appUser.GetByPermission(AppPermission.All))
                        {
                            Emailer.Send(x.Email, model.Code + " " + model.FullName + " employee created by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "New Employee Created");
                        }
                        foreach (var x in appUser.GetByPermission(AppPermission.Contact))
                        {
                            Emailer.Send(x.Email, model.Code + " " + model.FullName + " employee created by " + CurrentUser.Code + " " + CurrentUser.FullName + ".", "New Employee Created");
                        }
                    }
                    if (AppSettings.GetVal<bool>("notification:Notify"))
                    {
                        foreach (var x in appUser.GetByPermission(AppPermission.All))
                        {
                            notify.Create(model.OfficeId, x.Id, model.Id, AppNotificationType.Alert, "New employee created", AppSettings.GetVal("notification:URL") + "/Secure/Employee/Details/" + model.Id, model.Code + " " + model.FullName + " employee created by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }

                        foreach (var x in appUser.GetByPermission(AppPermission.Contact))
                        {
                            notify.Create(model.OfficeId, x.Id, model.Id, AppNotificationType.Alert, "New employee created", AppSettings.GetVal("notification:URL") + "/Secure/Employee/Details/" + model.Id, model.Code + " " + model.FullName + " employee created by " + CurrentUser.Code + " " + CurrentUser.FullName + ".");
                        }
                    }
                    #endregion

                    realtime.UpdateEmployees("New employee created.");

                    TempData["SuccessMsg"] = "New employee has been created successfully. An email has also been sent to the application user's email address.";
                }
                else
                {
                    model.UpdatedByUserId = CurrentUser.Id;
                    model.Username = model.Code;
                    appUser.Update(model);

                    #region Extra Fields
                    if (model.Extras != null)
                    {
                        appUser.DeleteExtraByAppUserId(model.Id);
                        foreach (var x in model.Extras)
                        {
                            var newExtra = extras.GetById(x.ExtraId);
                            if (newExtra.FieldType == "checkbox")
                            {
                                if (string.IsNullOrEmpty(x.Value) || x.Value == "no")
                                {
                                    x.Value = "false";
                                }
                            }
                            x.AppUserId = model.Id;
                            appUser.CreateExtraField(x);
                        }
                    }
                    #endregion

                    #region Employment
                    model.Employment.AppUserId = model.Id;
                    appUser.UpdateEmployment(model.Employment);
                    #endregion

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Employee Management", model.Code + " " + model.FullName + " Employee Updated", "~/Secure/Employee/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.FullName + "</strong> employee updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateEmployees("Employee has been updated.");

                    TempData["SuccessMsg"] = "Employee has been updated successfully.";
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
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Employee Management", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Employee/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Settings

        #region Manage Employee Data Fields
        
        public ActionResult Settings()
        {
            var model = extras.GetByModule("Employee");
            foreach (var x in model)
            {
                x.Fields = extras.GetByParentId(x.Id);
            }
            return View(model);
        }

        #region Sections
        [HttpPost]
        public JsonResult AddSection(Guid Id, string Section)
        {
            extras.Create(new ExtraFieldSection { Id = Id, Module = "Employee", Section = Section });
            return Json(true);
        }

        [HttpPost]
        public JsonResult UpdateSection(Guid Id, string Section)
        {
            extras.Update(new ExtraFieldSection { Id = Id, Section = Section });
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteSection(Guid Id)
        {
            var model = extras.GetByParentId(Id);
            if (model != null)
            {
                if (model.Count() > 0)
                {
                    foreach (var x in model)
                    {
                        appUser.DeleteExtraByExtraId(x.Id);
                    }
                }
            }
            appUser.DeleteExtraByExtraId(Id);
            extras.DeleteByParentId(Id);
            extras.Delete(Id);
            return Json(true);
        }
        #endregion

        #region Fields
        [HttpPost]
        public JsonResult AddField(Guid ParentId, Guid Id, string FieldType, string FieldName)
        {
            extras.Create(new ExtraFieldSection { Id = Id, Module = "Employee", ParentId = ParentId, FieldType = FieldType, FieldName = FieldName });
            return Json(true);
        }

        [HttpPost]
        public JsonResult UpdateField(Guid Id, string FieldType, string FieldName)
        {
            extras.Update(new ExtraFieldSection { Id = Id, FieldType = FieldType, FieldName = FieldName });
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteField(Guid Id)
        {
            extras.Delete(Id);
            return Json(true);
        }
        #endregion

        #endregion

        #region Timesheet Settings
        public ActionResult TimesheetSettings()
        {
            return View();
        }

        #region Pto Codes
        public PartialViewResult _AllPtoCodes()
        {
            var model = ptoCode.GetAll();
            return PartialView(model);
        }

        public PartialViewResult _PtoAssignees(Guid? Id)
        {
            var allEmployees = appUser.GetAll();
            foreach (var x in allEmployees)
            {
                if (x.OfficeId.HasValue)
                {
                    x.Office = ofcRepo.GetById(x.OfficeId.Value);
                }
            }
            ViewBag.AllEmployees = allEmployees;

            var model = new PtoCode();
            if (Id.HasValue)
            {
                model = ptoCode.GetById(Id.Value);
                model.Assignees = ptoCode.GetAssigneesByPtoId(Id.Value);
            }
            else
            {
                model.Assignees = new List<PtoCodeAssignee>();
            }
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult AddPtoRecord(string Title, bool IsAssignedToAll = false)
        {
            ptoCode.Create(new PtoCode { Title = Title, IsAssignedToAll = IsAssignedToAll });
            return Json(true);
        }
        [HttpPost]
        public JsonResult UpdateAssignedPtoRecord(Guid Id)
        {
            ptoCode.UpdateAssignedToAll(Id);
            return Json(true);
        }
        [HttpPost]
        public ActionResult PtoAssignees(PtoCode model)
        {
            ptoCode.DeleteAssigneesByPtoId(model.Id);
            if (model.Assignees != null)
            {
                foreach (var x in model.Assignees)
                {
                    if (!x.AppUserId.IsEmpty())
                    {
                        ptoCode.CreateAssignee(new PtoCodeAssignee { PtoId = model.Id, AppUserId = x.AppUserId });
                    }
                }
            }
            return RedirectToAction("TimesheetSettings");
        }
        [HttpPost]
        public JsonResult DeletePtoRecord(Guid Id)
        {
            ptoCode.DeleteAssigneesByPtoId(Id);
            ptoCode.Delete(Id);
            return Json(true);
        }
        #endregion

        #endregion

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveSettings(FormCollection form, string returnUrl)
        {
            try
            {
                var settings = form.AllKeys.ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Employee Settings", "Employee Settings Updated", "~/Secure/Employee/SaveSettings > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Employee settings updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Settings have been saved successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Employee Settings", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Employee/SaveSettings > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SaveOvertimeRuleSettings(FormCollection form, string OvertimeMethod, string OvertimeWeekly, string OvertimeDaily)
        {
            try
            {
                var settings = form.AllKeys.ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);
                
                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Employee Settings", "Employee Settings Updated", "~/Secure/Employee/SaveSettings > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Employee settings updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Settings have been saved successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Employee Settings", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Employee/SaveSettings > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Content("<script>window.location.href = '/Secure/Setting/Employees'</script>");
            //return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveApprovalSettings(bool Approval = false)
        {
            AppSettings.SetVal("timesheet:Approval", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveGeolocationSettings(bool Geolocation = false)
        {
            AppSettings.SetVal("timesheet:Geolocation", Geolocation.ToString().ToLowerInvariant());
            return Json(true);
        }

        #endregion

        #region Timesheet
        public PartialViewResult _AllTimesheets(Guid Id)
        {
            var model = timesheet.GetByReferenceId(Id);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult SaveIsOvertimeRuleSettings(bool OvertimeRule = false)
        {
            if(OvertimeRule == true)
            {
                AppSettings.SetVal("employee:OvertimeRules", OvertimeRule.ToString().ToLowerInvariant());
                AppSettings.SetVal("employee:OvertimeRulesIsCustom", "false");
            }
            else
            {
                AppSettings.SetVal("employee:OvertimeRules", OvertimeRule.ToString().ToLowerInvariant());
                AppSettings.SetVal("employee:OvertimeRulesIsCustom", "true");
            }
            return Json(true);
        }
        #endregion

        public ActionResult _OvertimeRules()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult SaveTimesheetPaidTimeOff(bool PaidTimeOff = false)
        {
            AppSettings.SetVal("timesheet:PaidTimeoff", PaidTimeOff.ToString().ToLowerInvariant());
            return Json(true);
        }

        public ActionResult _EmployeeDataFields()
        {
            var model = extras.GetByModule("Employee");
            foreach (var x in model)
            {
                x.Fields = extras.GetByParentId(x.Id);
            }
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult SaveCustomDataFields(bool CustomFields = false)
        {
            AppSettings.SetVal("employee:CustomDataFields", CustomFields.ToString().ToLowerInvariant());
            return Json(true);
        }

        [HttpPost]
        public JsonResult SaveIsFLSAStandardSetting(bool FlsaSetting = false)
        {
            AppSettings.SetVal("payroll:IsFLSAStandard", FlsaSetting.ToString().ToLowerInvariant());
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _OvertimeRates()
        {
            return PartialView();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult SaveOvertimeRatesSettings(FormCollection form)
        {
            try
            {
                var settings = form.AllKeys.ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Overtime Rates Settings", "Overtime Rates Settings Updated", "~/Secure/Setting/Employees > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Overtime Rates settings updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Settings have been saved successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Overtime Rates Settings", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/Employees > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Content("<script>window.location.href = '/Secure/Setting/Employees'</script>");
        }

        public ActionResult PayrollSettings()
        {
            return View();
        }

        public ActionResult Employees()
        {
            return View();
        }

        public ActionResult TimeSheets()
        {
            return View();
        }

        public PartialViewResult _EmployeeInsurance()
        {
            var model = insuranceRepo.SelectAll();
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult DeleteInsuranceRecord(Guid Id)
        {
            insuranceRepo.Delete(Id);
            return Json(true);
        }
    }
}