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
using Ecommerce.Logic;
using Ecommerce.Entity;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize(AppPermission.All, AppPermission.ViewEcommerce, AppPermission.Ecommerce)]
    public class EcommerceController : AppController
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
        private IShippingManagement shippers;

        public EcommerceController()
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
            shippers = db.As<IShippingManagement>();

            ViewBag.AllOffices = ofcRepo.GetAll();
        }

        #region Settings

        #region Ecommerce General Settings
        [HttpPost]
        public JsonResult SaveIncludeVATforTraderSettings(bool Approval = false)
        {
            AppSettings.SetVal("inventory:includeVATforTrader", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveIncludeVATforRetailerSettings(bool Approval = false)
        {
            AppSettings.SetVal("inventory:IncludeVATforRetailer", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveShippingFreeForAllSettings(bool Approval = false)
        {
            AppSettings.SetVal("Shipping:FreeForAll", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveShippingFreeForEconomySettings(bool Approval = false)
        {
            AppSettings.SetVal("Shipping:FreeForEconomy", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveShippingFreeForStandardSettings(bool Approval = false)
        {
            AppSettings.SetVal("Shipping:FreeForExpress", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveShippingFreeForExpressSettings(bool Approval = false)
        {
            AppSettings.SetVal("Shipping:FreeForStandard", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveOutOfStockSettings(bool Approval = false)
        {
            AppSettings.SetVal("inventory:OutOfStock", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveMinimumOrderQuantity(bool Approval = false)
        {
            AppSettings.SetVal("MinimumOrderQuantity", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        #endregion

        #region Shipping Configuration
        [HttpPost]
        public JsonResult SaveCashOnDeliverySettings(bool Approval = false)
        {
            AppSettings.SetVal("Shipping:CashOnDelivery", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveOnlineOrderSettings(bool Approval = false)
        {
            AppSettings.SetVal("Shipping:OnlineOrder", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveClickAndCollectSettings(bool Approval = false)
        {
            AppSettings.SetVal("Shipping:ClickAndCollect", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveOnlinePaymentSettings(bool Approval = false)
        {
            AppSettings.SetVal("ClickAndCollect:OnlinePayment", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SavePaymentAtPickupSettings(bool Approval = false)
        {
            AppSettings.SetVal("ClickAndCollect:PaymentAtPickup", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveBankPaymentSettings(bool Approval = false)
        {
            AppSettings.SetVal("ClickAndCollect:BankPayment", Approval.ToString().ToLowerInvariant());
            return Json(true);
        }
        [HttpPost]
        public JsonResult SavePaymentGateway(string value)
        {
            AppSettings.SetVal("PaymentGateway", value.Trim());
            return Json(true);
        }
        #endregion

        #region Json Requests
        public JsonResult GetAllCompanies()
        {
            var pages = shippers.GetAllCompanies().ToList();
            return Json(pages, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetShippingPackages(long Id)
        {
            var result = new List<ShippingPackageModel>();
            if (Id > 0)
            {
                 result = shippers.GetCompanyPackages(Id).ToList();
            }
            else
                 result = shippers.GetAllPackages().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllPostcodes()
        {
            var pages = shippers.GetAllPostcodes().ToList();
            return Json(pages, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Shipping Companies

        public ActionResult _ShippingCompanyRecord(long? Id)
        {
            var model = new ShippingCompanyModel();
            if (Id > 0)
            {
                model = shippers.GetShipCompanyById(Id.Value);
            }
            return PartialView(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ShippingCompanyRecord(ShippingCompanyModel model)
        {
            try
            {
                if (model.ShippingCompanyId == 0)
                {
                    shippers.Create(model);
                    if (model.ShippingStampImage != null && model.ShippingStampImage.ContentLength > 0)
                    {
                        var path = Server.MapPath("~/Content/Uploads/Ecommerce/");
                        model.ShippingStampImage.SaveAs(string.Format(@"{0}sc_{1}.jpg", path, model.ShippingCompanyId));
                    }
                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page created", "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> page created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    //realtime.UpdateCmsPages("New page has been created successfully.");
                    TempData["SuccessMsg"] = "Shipping Company has been created successfully.";
                }
                else
                {
                    shippers.Update(model);
                    if (model.ShippingStampImage != null && model.ShippingStampImage.ContentLength > 0)
                    {
                        var path = Server.MapPath("~/Content/Uploads/Ecommerce/");
                        model.ShippingStampImage.SaveAs(string.Format(@"{0}sc_{1}.jpg", path, model.ShippingCompanyId));
                    }
                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page updated", "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> page updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    //realtime.UpdateCmsPages("Shipping Company has been updated successfully.");
                    TempData["SuccessMsg"] = "Shipping Company has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Content(@"<script language='javascript' type='text/javascript'> location.href = '/Secure/Setting/Ecommerce'; </script>");
        }

        public ActionResult _ShippingCompany(long? Id)
        {
            var model = new ShippingCompanyModel();
            if (Id > 0)
            {
                model = shippers.GetShipCompanyById(Id.Value);
            }
            return PartialView(model);
        }
        [HttpPost]
        public JsonResult ShippingCompanyDeleteAble(long id)
        {
            return ((shippers.IsDeleteAble(id) > 0) ? Json(true) : Json(false));
        }
        [HttpPost]
        public JsonResult ShippingCompanyDelete(long id)
        {
            try
            {
                if (shippers.IsDeleteAble(id) > 0)
                {
                    return Json(false);
                }
                else
                {
                    shippers.Delete(id);
                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebContent", "Content deleted", "~/CMS/WebContent/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Content deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    //realtime.UpdateCmsContents("Shipping Company has been deleted.");

                    TempData["SuccessMsg"] = "Shipping Company has been deleted successfully.";
                }
            }
            catch (Exception ex)
            {

                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebContent", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebContent/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        [HttpPost]
        public JsonResult DeleteMultipleCompanies(string Ids)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        shippers.Delete(Convert.ToInt32(x));
                    }
                }

                #region Activity Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Multiple pages deleted", "~/CMS/WebPage/DeleteMultipleSections > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple pages deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                //realtime.UpdateCmsPages("Multiple pages deleted.");

                TempData["SuccessMsg"] = "Selected Companies deleted.";
            }
            catch (Exception ex)
            {
                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/DeleteMultipleSections > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Shipping Packages
        public ActionResult _ShippingPackageRecord(long? Id)
        {
            var model = new ShippingPackageModel();
            if (Id > 0)
            {
                model = shippers.GetShipPackageById(Id.Value);
            }
            ViewBag.PackageTypeList = new SelectList(Enum.GetValues(typeof(PackageType)).Cast<PackageType>().Select(x => new { value = x.ToString(), text = x.ToString() }).ToList(), "value", "text");
            ViewBag.ShippingCompanyList = new SelectList(shippers.GetAllCompanies().Select(x => new { value = x.ShippingCompanyId, text = x.Title }).ToList(), "value", "text");
            return PartialView(model);
        }
        public ActionResult _ShippingPackage(long? Id)
        {
            var model = new ShippingPackageModel();
            if (Id > 0)
            {
                model = shippers.GetShipPackageById(Id.Value);
            }
            ViewBag.PackageTypeList = new SelectList(Enum.GetValues(typeof(PackageType)).Cast<PackageType>().Select(x => new { value = x.ToString(), text = x.ToString() }).ToList(), "value", "text");
            ViewBag.ShippingCompanyList = new SelectList(shippers.GetAllCompanies().Select(x => new { value = x.ShippingCompanyId, text = x.Title }).ToList(), "value", "text");
            return PartialView(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult ShippingPackageRecord(ShippingPackageModel model)
        {
            try
            {
                if (model.ShippingCompanyId == 0)
                {
                    shippers.CreateShipPackage(model);
                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page created", "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> page created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    //realtime.UpdateCmsPages("New Package has been created successfully.");
                    TempData["SuccessMsg"] = "Shipping Package has been created successfully.";
                }
                else
                {
                    shippers.UpdateShipPackage(model);
                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page updated", "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> page updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    //realtime.UpdateCmsPages("Shipping Package has been updated successfully.");
                    TempData["SuccessMsg"] = "Shipping Package has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Content(@"<script language='javascript' type='text/javascript'> location.href = '/Secure/Setting/Ecommerce'; </script>");
        }

        [HttpPost]
        public JsonResult ShippingPackageDelete(long id)
        {
            try
            {
                if (shippers.IsPackageDeleteAble(id) > 0)
                {
                    return Json(false);
                }
                else
                {
                    shippers.DeletePackage(id);
                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebContent", "Content deleted", "~/CMS/WebContent/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Content deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    //realtime.UpdateCmsContents("Shipping Package has been deleted.");

                    TempData["SuccessMsg"] = "Shipping Package has been deleted successfully.";
                }
            }
            catch (Exception ex)
            {

                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebContent", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebContent/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        public ActionResult _ShippingPostcodeRecord(long? id)
        {
            var model = new PostcodeModel();
            if (id > 0)
            {
                model = shippers.GetPostcodeById(id.Value);
            }
            ViewBag.RestrictionLevel = Enum.GetValues(typeof(PostcodeRestrictedLevel)).Cast<PostcodeRestrictedLevel>().Select(x => new { value = x, text = x.ToString().ToSpacedTitleCase() });
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult ShippingPostcodeRecord(PostcodeModel model)
        {
            try
            {
                if (model.Id == 0)
                {
                    shippers.CreatePostcode(model);
                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page created", "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> page created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    //realtime.UpdateCmsPages("Shipping PostCode has been created successfully.");
                    TempData["SuccessMsg"] = "Shipping PostCode has been created successfully.";
                }
                else
                {
                    shippers.UpdatePostcode(model);
                    #region Activity Log
                    //appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Cms - WebPage", "Page updated", "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td><strong>" + model.Name + "</strong> page updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    //realtime.UpdateCmsPages("Shipping PostCode has been updated successfully.");
                    TempData["SuccessMsg"] = "Shipping PostCode has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebPage", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebPage/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Content(@"<script language='javascript' type='text/javascript'> location.href = '/Secure/Setting/Ecommerce'; </script>");
        }

        [HttpPost]
        public JsonResult IsShippingPostcodeDelable(long id)
        {
            return ((shippers.IsDeleteAble(id) > 0) ? Json(true) : Json(false));
        }
        [HttpPost]
        public JsonResult ShippingPostcodeDelete(long id)
        {
            try
            {
                shippers.DeletePostCode(id);
                #region Activity Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebContent", "Content deleted", "~/CMS/WebContent/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Content deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                //realtime.UpdateCmsContents("Shipping PostCode has been deleted.");

                TempData["SuccessMsg"] = "Shipping PostCode has been deleted successfully.";
            }
            catch (Exception ex)
            {

                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebContent", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebContent/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Payment Gateway Settings

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult SavePaymentGatewaySettings(FormCollection form, string returnUrl)
        {
            try
            {
                var settings = form.AllKeys.ToDictionary(k => k, v => form[v]);
                AppSettings.SetVal(settings);

                #region Activity Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Settings", "Settings Updated", "~/Secure/Setting/Index > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Settings updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion
                //TempData["SuccessMsg"] = "Settings have been saved successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                //appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Settings", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/Setting/Index > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                //TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return string.IsNullOrEmpty(returnUrl) ? View() : RedirectToLocal(returnUrl);
            //return Content("<script> location.reload(true); </script>");
        }
        #endregion

    }
}