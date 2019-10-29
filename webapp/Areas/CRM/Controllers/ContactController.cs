using BaseApp.Entity;
using BaseApp.System;
using CRM.Entity;
using CRM.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using BaseApp.Logic;
using WebApp.Hubs;
using Ecommerce.Entity;
using Ecommerce.Logic;

namespace WebApp.Areas.CRM.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All)]
    public class ContactController : AppController
    {
        private IAppMessage appMsg;

        RealTimeHub realtime = new RealTimeHub();

        private IContact cntct;

        private IAppLog appLog;
        private IAppUser appUser;
        private IOrder orders;

        public ContactController()
        {
            appMsg = db.As<IAppMessage>();
            cntct = db.As<IContact>();
            appLog = db.As<IAppLog>();
            appUser = db.As<IAppUser>();
            orders = db.As<IOrder>();
        }

        #region Leads
        public ActionResult Index()
        {
            var model = cntct.GetAll();
            ViewBag.AppUsers = appUser.GetAll();
            return View(model);
        }
        public ActionResult Customers(int Id)
        {
            var model = cntct.GetAll().Where(x => x.ContactType == ContactType.Customer).ToList();
            return View(model);
        }
        public ActionResult Suppliers()
        {
            var model = cntct.GetAll().Where(x => x.ContactType == ContactType.Supplier).ToList();
            return View(model);
        }
        public ActionResult Traders()
        {
            var model = cntct.GetAll().Where(x => x.ContactType == ContactType.Trader || x.ContactType == ContactType.TraderDropShipper).ToList();
            return View(model);
        }
        public ActionResult Dropshipers()
        {
            var model = cntct.GetAll().Where(x => x.ContactType == ContactType.DropShipper || x.ContactType == ContactType.TraderDropShipper).ToList();
            return View(model);
        }

        public ActionResult Record(Guid? Id)
        {
            var model = new Contact();
           if (Id.HasValue)
            {
                model = cntct.GetById(Id.Value);
                model.Company = cntct.GetByContactId(Id.Value);
                model.MorePhones = cntct.GetAllPhones(Id.Value);
                model.MoreEmails = cntct.GetAllEmails(Id.Value);
                model.MoreAddresses = cntct.GetAllAddresses(Id.Value);
                ViewBag.ContactTypes = Enum.GetValues(typeof(ContactType)).Cast<ContactType>().Select(x => new { value = x, text = x.ToString().ToSpacedTitleCase() });
                model.SelectedContactTypes = cntct.GetByContact(model.Id);
            }
            else
            {
                model.Code = cntct.GetMaxCode();
                model.DateOfBirth = DateTime.UtcNow.AddYears(-20);
                model.Company = new ContactCompany();
                model.MorePhones = new List<ContactPhone>();
                model.MoreEmails = new List<ContactEmail>();
                model.MoreAddresses = new List<ContactAddress>();
                model.SelectedContactTypes = new List<CrmContactType>();
                ViewBag.ContactTypes = Enum.GetValues(typeof(ContactType)).Cast<ContactType>().Select(x => new { value = x, text = x.ToString().ToSpacedTitleCase() });
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Record(Contact model, HttpPostedFileBase file, string SaveClick, int type)
        {
            model.CreatedByUserId = model.UpdatedByUserId = CurrentUser.Id;
            model.Status = ContactStatus.PendingApproval;
            model.ContactType = (type == 5) ? model.ContactType : (ContactType)type;
            try
            {
                if (model.Id.IsEmpty())
                {
                    /*model.Username =*/
                    model.Password = model.Code;
                    model.Id = cntct.Create(model);
                    if (model.Company != null)
                    {
                        model.Company.ContactId = model.Id;
                        cntct.CreateCompany(model.Company);
                    }

                    #region Adding More Emails
                    if (model.MoreEmails != null)
                    {
                        foreach (var x in model.MoreEmails)
                        {
                            if (!string.IsNullOrEmpty(x.Email))
                            {
                                x.ContactId = model.Id;
                                cntct.CreateMoreEmail(x);
                            }
                        }
                    }
                    #endregion

                    #region Adding More Phones
                    if (model.MorePhones != null)
                    {
                        foreach (var x in model.MorePhones)
                        {
                            if (!string.IsNullOrEmpty(x.Phone))
                            {
                                x.ContactId = model.Id;
                                cntct.CreateMorePhone(x);
                            }
                        }
                    }
                    #endregion

                    #region Adding More Address
                    if (model.MoreAddresses != null)
                    {
                        foreach (var x in model.MoreAddresses)
                        {
                            if (!string.IsNullOrEmpty(x.AddressLine1) && !string.IsNullOrEmpty(x.City) && !string.IsNullOrEmpty(x.State) && !string.IsNullOrEmpty(x.ZipCode) && !string.IsNullOrEmpty(x.Country))
                            {
                                var addressModel = new ContactAddress();
                                addressModel.ContactId = model.Id;
                                addressModel.AddressLine1 = x.AddressLine1;
                                addressModel.AddressLine2 = x.AddressLine2;
                                addressModel.City = x.City;
                                addressModel.State = x.State;
                                addressModel.ZipCode = x.ZipCode;
                                addressModel.Country = x.Country;
                                cntct.CreateMoreAddress(addressModel);
                            }
                        }
                    }
                    #endregion

                    #region Contact Types

                    if (model.ContactTypes != null)
                    {
                        var SavedTypes = cntct.GetByContact(model.Id);
                        //delete existing 
                        if (SavedTypes.Count > 0)
                            cntct.DeleteTypes(model.Id);
                        //Saving Database
                        foreach (var item in model.ContactTypes)
                        {
                            int value = (int)Enum.Parse(typeof(ContactType), item.Replace(" ", String.Empty));
                            cntct.CreateTypes((ContactType)value, model.Id);
                        }
                    }
                    #endregion

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "CRM - Lead", model.FullName + " lead created", "~/CRM/Contact/LeadRecord > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.FullName + "</strong> lead created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    #region EmailSending

                    Emailer.Send(model.Email, EmailTemplateType.ContactSignup, model);

                    var appMessageModel = new AppMessage();
                    appMessageModel.Email = model.Email;
                    appMessageModel.Status = AppMessageStatus.Outbox;
                    appMessageModel.Subject = EmailTemplateType.ContactSignup;
                    appMessageModel.Message = EmailTemplateType.ContactSignup;
                    TempData["SuccessMsg"] = "Message has been sent successfully.";
                    realtime.UpdateMessages(null);
                    appMessageModel.CreatorId = appMessageModel.ContactId = CurrentUser.Id;
                    appMessageModel.Id = appMsg.Create(appMessageModel);

                    //var cntact = appUser.GetUserById(new Guid(x));
                    appMsg.Create(null, appMessageModel.Id, appMessageModel.CreatorId, null, AppMessageStatus.Inbox, EmailTemplateType.ContactSignup, model.Id, null);

                    realtime.UpdateMessages(null);
                    #endregion

                    TempData["SuccessMsg"] = model.FullName + " has been created successfully. Email Sent...";
                }
                else
                {
                    cntct.Update(model);
                    if(model.Company != null)
                        cntct.UpdateCompany(model.Company);

                    #region Adding More Emails
                    if (model.MoreEmails != null)
                    {
                        cntct.DeleteMoreEmails(model.Id);
                        foreach (var x in model.MoreEmails)
                        {
                            if (!string.IsNullOrEmpty(x.Email))
                            {
                                x.ContactId = model.Id;
                                cntct.CreateMoreEmail(x);
                            }
                        }
                    }
                    else
                    {
                        cntct.DeleteMoreEmails(model.Id);
                    }
                    #endregion

                    #region Adding More Phones
                    if (model.MorePhones != null)
                    {
                        cntct.DeleteMorePhones(model.Id);
                        foreach (var x in model.MorePhones)
                        {
                            if (!string.IsNullOrEmpty(x.Phone))
                            {
                                x.ContactId = model.Id;
                                cntct.CreateMorePhone(x);
                            }
                        }
                    }
                    else
                    {
                        cntct.DeleteMorePhones(model.Id);
                    }
                    #endregion

                    #region Adding More Address
                    if (model.MoreAddresses != null)
                    {
                        cntct.DeleteMoreAddress(model.Id);
                        foreach (var x in model.MoreAddresses)
                        {
                            if (!string.IsNullOrEmpty(x.AddressLine1) && !string.IsNullOrEmpty(x.City) && !string.IsNullOrEmpty(x.State) && !string.IsNullOrEmpty(x.ZipCode) && !string.IsNullOrEmpty(x.Country))
                            {
                                var addressModel = new ContactAddress();
                                addressModel.ContactId = model.Id;
                                addressModel.AddressLine1 = x.AddressLine1;
                                addressModel.AddressLine2 = x.AddressLine2;
                                addressModel.City = x.City;
                                addressModel.State = x.State;
                                addressModel.ZipCode = x.ZipCode;
                                addressModel.Country = x.Country;
                                cntct.CreateMoreAddress(addressModel);
                            }
                        }
                    }
                    #endregion

                    #region Contact Types

                    if (model.ContactTypes != null)
                    {
                        var SavedTypes = cntct.GetByContact(model.Id);
                        //delete existing 
                        if (SavedTypes.Count > 0)
                            cntct.DeleteTypes(model.Id);
                        //Saving Database
                        foreach (var item in model.ContactTypes)
                        {
                            int value = (int)Enum.Parse(typeof(ContactType), item.Replace(" ", String.Empty));
                            cntct.CreateTypes((ContactType)value, model.Id);
                        }
                    }
                    #endregion

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "CRM - Lead", model.FullName + " lead updated", "~/CRM/Contact/LeadRecord > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.FullName + "</strong> lead updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = model.FullName + " has been updated successfully.";
                }

                var dpPath = Server.MapPath("~/Content/Uploads/CRM/");
                if (file.HasValue())
                {
                    file.SaveAs(dpPath + model.Id + ".jpg");
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "CRM - Lead", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/FileManager/Index > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            if(SaveClick == "SaveAddNew")
            {
                return RedirectToAction("Record");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Contact
        public ActionResult Summary(Guid Id)
        {
            var model = cntct.GetById(Id);
            model.Company = cntct.GetByContactId(Id);
            model.MoreEmails = cntct.GetAllEmails(Id);
            model.MorePhones = cntct.GetAllPhones(Id);
            model.MoreAddresses = cntct.GetAllAddresses(Id);
            model.SelectedContactTypes = cntct.GetByContact(model.Id);
            ViewBag.SalesGrid = orders.GetSaleOrders((int)OrderType.Sale, (int)OrderType.SalesReturn, (int)OrderType.SalesExchange).Where(x => x.ContactId == Id).ToList();
            ViewBag.POList = orders.GetPOList((int)OrderType.Purchase).Where(x => x.ContactId == Id).ToList();
            ViewBag.OrderDetails = orders.GetOrderDetails();
            return View(model);
        }
        #endregion

        #region Remote Functions
        [HttpPost]
        public JsonResult CheckDuplicateEmail(Guid Id, string Email)
        {
            var checkEmail = cntct.CheckDuplicateEmail(Id, Email);
            if (checkEmail == null)
            {
                return Json(true);
            }
            else
            {
                return Json("Email already exists.");
            }
        }

        [HttpPost]
        public JsonResult Delete(Guid Id)
        {
            try
            {
                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, Id, CurrentUser.Id, AppLogType.Activity, "CRM", "Contact Deleted", "~/CRM/Contact/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Contact deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                cntct.DeleteMorePhones(Id);
                cntct.DeleteMoreEmails(Id);
                cntct.Delete(Id);

                TempData["SuccessMsg"] = "Contact has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "CRM", ex.GetType().Name.ToSpacedTitleCase(), "~/CRM/Contact/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                        cntct.DeleteMorePhones(new Guid(x));
                        cntct.DeleteMoreEmails(new Guid(x));
                        cntct.Delete(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "CRM", "Multiple Contacts Deleted", "~/CRM/Contact/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple contacts deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                TempData["SuccessMsg"] = "Selected contacts has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "CRM", ex.GetType().Name.ToSpacedTitleCase(), "~/CRM/Contact/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost, AppAuthorize(AppPermission.All, AppPermission.Contact)]
        public JsonResult ChangeStatus(Guid Id)
        {
            try
            {
                var model = cntct.GetById(Id);
                if (model.Status == ContactStatus.PendingApproval)
                    cntct.ChangeStatus(Id, ContactStatus.Active, CurrentUser.Id);
                else if(model.Status == ContactStatus.Active)
                        cntct.ChangeStatus(Id, ContactStatus.Suspended, CurrentUser.Id);
                else if (model.Status == ContactStatus.Suspended)
                    cntct.ChangeStatus(Id, ContactStatus.Active, CurrentUser.Id);

                #region Activity Log
                //appLog.Create(model.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Access Control", model.Code + " " + model.FullName + " Contact Status Changed", "~/Secure/AccessControl/ChangeStatus > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Code + " " + model.FullName + "</strong> contact status changed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateAppUsers("The status of contact has been updated.");

                TempData["SuccessMsg"] = "The status of contact has been updated successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Access Control", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/AccessControl/ChangeStatus > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion
    }
}