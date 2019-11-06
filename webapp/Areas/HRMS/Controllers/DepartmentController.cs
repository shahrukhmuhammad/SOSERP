using BaseApp.Entity;
using BaseApp.System;
using HRMS;
using HRMS.Model;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Areas.HRM;

namespace WebApp.Areas.HRMS.Controllers
{
    //[AppAuthorize(AppPermission.All, AppPermission.ViewHRMS, AppPermission.HRMS)]
    public class DepartmentController : AppController
    {
        private DepartmentEntity departRepo;

        public DepartmentController()
        {
            departRepo = new DepartmentEntity();
        }
        public ActionResult Deparments()
        {
            return View();
        }
        public PartialViewResult _AllDeparments()
        {
            var model = departRepo.GetAllDepartments();
            return PartialView(model);
        }

        #region Details
        public ActionResult Details(Guid Id)
        {
            var model = new HRM_Vew_Employee();
            //model = employeeRepo.GetUserById(Id);
            //ViewBag.References = employeeRepo.GetReferencesByEmpId(Id);
            return View(model);
        }
        #endregion
        #region Record
        public ActionResult Record(Guid? Id)
        {
            var model = new HRM_Vew_Employee();
            //if (Id.HasValue)
            //{
            //    model = employeeRepo.GetUserById(Id.Value);
            //    ViewBag.References = employeeRepo.GetReferencesByEmpId(Id.Value);
            //}
            //else
            //{
            //    model.Code = employeeRepo.GetNextEmployeeCode();
            //}
            //ViewBag.RegionList = new SelectList(regionRepo.GetAllRegionsDropdown(), "Value", "Text");

            return View(model);
        }

        public ActionResult Index()
        {

            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Record(HRM_Vew_Employee model, EmployeePostedFiles files, DocumentPostedFiles documentPostedFiles, List<EmpReference> Reference)
        {
            try
            {
                if (model.EmployeeId.IsEmpty())
                {
                    model.CreatedBy = CurrentUser.Id;
                    model.CreatedOn = DateTime.Now;
                    model.ProfileStatus = Convert.ToByte(ProfileStatus.Draft);
                    model.Status = Convert.ToByte(EmployeeStatus.Active);
                    //var res = employeeRepo.Create(model);
                    //if (res.HasValue)
                    //{
                    //    model.EmployeeId = res.Value;
                    //}

                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "CRM - Lead", model.FullName + " lead created", "~/CRM/Contact/LeadRecord > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.FullName + "</strong> lead created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region EmailSending

                    //Emailer.Send(model.Email, EmailTemplateType.ContactSignup, model);

                    //var appMessageModel = new AppMessage();
                    //appMessageModel.Email = model.Email;
                    //appMessageModel.Status = AppMessageStatus.Outbox;
                    //appMessageModel.Subject = EmailTemplateType.ContactSignup;
                    //appMessageModel.Message = EmailTemplateType.ContactSignup;
                    //TempData["SuccessMsg"] = "Message has been sent successfully.";
                    //realtime.UpdateMessages(null);
                    //appMessageModel.CreatorId = appMessageModel.ContactId = CurrentUser.Id;
                    //appMessageModel.Id = appMsg.Create(appMessageModel);

                    ////var cntact = appUser.GetUserById(new Guid(x));
                    //appMsg.Create(null, appMessageModel.Id, appMessageModel.CreatorId, null, AppMessageStatus.Inbox, EmailTemplateType.ContactSignup, model.Id, null);

                    //realtime.UpdateMessages(null);
                    #endregion

                    TempData["SuccessMsg"] = model.Name + " has been created successfully.";
                }
                else
                {
                    //Utils.IsFileExist("");
                    model.UpdatedBy = CurrentUser.Id;
                    model.UpdatedOn = DateTime.Now;
                    bool res = true; //employeeRepo.Update(model);


                    if (res)
                    {
                        TempData["SuccessMsg"] = model.Name + " has been updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
                    }
                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "CRM - Lead", model.FullName + " lead updated", "~/CRM/Contact/LeadRecord > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.FullName + "</strong> lead updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                }
            }
            catch (Exception ex)
            {
                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "CRM - Lead", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/FileManager/Index > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            //if (SaveClick == "SaveAddNew")
            //{
            //    return RedirectToAction("Record");
            //}
            //else
            //{
            return RedirectToAction("Employees");
        }
        #endregion

        #region Remote Functions
        [HttpPost]
        public JsonResult CheckDuplicateEmail(Guid Id, string CNIC)
        {
            var checkEmail = true; //employeeRepo.CheckDuplicateEmail(Id, CNIC);
            if (!checkEmail)
            {
                return Json(true);
            }
            else
            {
                return Json("Email already exists.");
            }
        }

        [HttpPost]
        public JsonResult GetCentersByRegionId(Guid Id)
        {
            return Json("");//centerRepo.GetCentersDropdown(Id));
        }
        [HttpPost]
        public JsonResult GetDesignationsByDepartId(Guid Id)
        {
            return Json("");//designationRepo.GetDesignationDropdown(Id));
        }
        #endregion

    }
}