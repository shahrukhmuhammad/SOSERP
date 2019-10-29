﻿using BaseApp.Entity;
using BaseApp.System;
using HRMS;
using HRMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.HRMS.Controllers
{
    [AppAuthorize(AppPermission.All, AppPermission.ViewHRMS, AppPermission.HRMS)]
    public class EmployeeController : AppController
    {
        private EmployeeEntity employeeRepo;

        public EmployeeController()
        {
            employeeRepo = new EmployeeEntity();
        }
        public ActionResult Employees()
        {
            return View();
        }
        public PartialViewResult _AllEmployees()
        {
            var model = employeeRepo.GetAllEmployees();
            return PartialView(model);
        }

        #region Employee Record
        public ActionResult Record(Guid? Id)
        {
            var model = new HRM_Vew_Employee();
            if (Id.HasValue)
            {
                model = employeeRepo.GetUserById(Id.Value);
                ViewBag.References = employeeRepo.GetReferencesByEmpId(Id.Value);
            }
            else
            {
                model.Code = employeeRepo.GetNextEmployeeCode();
            }
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Record(HRM_Vew_Employee model, EmployeePostedFiles files, HttpPostedFileBase[] documentFiles, List<EmpReference> Reference)
        {
            try
            {
                if (model.EmployeeId.IsEmpty())
                {
                    model.CreatedBy = CurrentUser.Id;
                    model.CreatedOn = DateTime.Now;
                    model.Status = Convert.ToByte(AppUserStatus.UnVerified);
                    var res = employeeRepo.Create(model);
                    if (res.HasValue)
                    {
                        model.EmployeeId = res.Value;
                    }

                    #region Add References
                    if (model.EmployeeId != null && Reference != null && Reference.Count > 0)
                    {
                        Reference.ForEach(i => {
                            i.ReferenceId = Guid.NewGuid();
                            i.EmployeeId = res.Value;
                            //i.RefAddressType = Convert.ToByte();
                            i.CreatedBy = CurrentUser.Id;
                            i.CreatedOn = DateTime.Now;

                        });
                        employeeRepo.AddOrUpdateEmpReference(Reference);
                    }
                    #endregion

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
                    model.UpdatedBy = CurrentUser.Id;
                    model.UpdatedOn = DateTime.Now;
                    bool res = employeeRepo.Update(model);
                    //    if (model.Company != null)
                    //        cntct.UpdateCompany(model.Company);

                    #region Update Reference Code
                    Reference.ForEach(i => {
                        i.EmployeeId = model.EmployeeId;
                        i.UpdatedBy = CurrentUser.Id;
                        i.UpdatedOn = DateTime.Now;

                    });
                    employeeRepo.AddOrUpdateEmpReference(Reference);


                    #endregion


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
                #region Files Upload
                var dpPath = Server.MapPath("~/Content/Uploads/HRMS/Dp/");
                if (files.ProfilePhoto.HasValue())
                {
                    files.ProfilePhoto.SaveAs(dpPath + model.EmployeeId + ".jpg");
                }
                if (files.Reference1Photo.HasValue())
                {
                    files.Reference1Photo.SaveAs(dpPath + "ref1-" + model.EmployeeId + ".jpg");
                }
                if (files.Reference2Photo.HasValue())
                {
                    files.Reference2Photo.SaveAs(dpPath + "ref2-" + model.EmployeeId + ".jpg");
                }
                if (documentFiles.Length > 0)
                {
                    var docDir = Server.MapPath("~/Content/Uploads/HRMS/EmployeeDocs/");
                    string docfileName;

                    foreach (var item in documentFiles)
                    {
                        if (item != null)
                        {
                            docfileName = $"{docDir}{model.EmployeeId}_{DateTime.Now.ToString("dd-MM-yyyy")}_{item.FileName}";
                            item.SaveAs(docfileName);
                        }
                        
                    }
                    //documentFiles.SaveAs(dpPath + "ref-" + model.EmployeeId + ".jpg");
                }
                #region Finger Print Upload
                var fingerDir = Server.MapPath("~/Content/Uploads/HRMS/FingerPrints/");
                if (files.LeftThumb.HasValue())
                {
                    files.LeftThumb.SaveAs($"{fingerDir}{model.EmployeeId}_LeftThumb.jpg");
                }
                if (files.LeftIndexFinger.HasValue())
                {
                    files.LeftIndexFinger.SaveAs($"{fingerDir}{model.EmployeeId}_LeftIndexFinger.jpg");
                }
                if (files.LeftMiddleFinger.HasValue())
                {
                    files.LeftMiddleFinger.SaveAs($"{fingerDir}{model.EmployeeId}_LeftMiddleFinger.jpg");
                }
                if (files.LeftRingFinger.HasValue())
                {
                    files.LeftRingFinger.SaveAs($"{fingerDir}{model.EmployeeId}_LeftRingFinger.jpg");
                }
                if (files.LeftBabyFinger.HasValue())
                {
                    files.LeftBabyFinger.SaveAs($"{fingerDir}{model.EmployeeId}_LeftBabyFinger.jpg");
                }

                if (files.RightThumb.HasValue())
                {
                    files.RightThumb.SaveAs($"{fingerDir}{model.EmployeeId}_RightThumb.jpg");
                }
                if (files.RightIndexFinger.HasValue())
                {
                    files.RightIndexFinger.SaveAs($"{fingerDir}{model.EmployeeId}_RightIndexFinger.jpg");
                }
                if (files.RightMiddleFinger.HasValue())
                {
                    files.RightMiddleFinger.SaveAs($"{fingerDir}{model.EmployeeId}_RightMiddleFinger.jpg");
                }
                if (files.RightRingFinger.HasValue())
                {
                    files.RightRingFinger.SaveAs($"{fingerDir}{model.EmployeeId}_RightRingFinger.jpg");
                }
                if (files.RightBabyFinger.HasValue())
                {
                    files.RightBabyFinger.SaveAs($"{fingerDir}{model.EmployeeId}_RightBabyFinger.jpg");
                }
                #endregion
                #endregion

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




    }
}