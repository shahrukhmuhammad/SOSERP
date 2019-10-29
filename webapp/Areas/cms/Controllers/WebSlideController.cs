using BaseApp.Entity;
using BaseApp.System;
using CMS.Logic;
using System;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using BaseApp.Logic;
using CMS.Entity;
using WebApp.Hubs;

namespace WebApp.Areas.CMS.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All, AppPermission.ViewCMS, AppPermission.CMS)]
    public class WebSlideController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private ICmsSlide webSlide;
        private IAppLog appLog;

        public WebSlideController()
        {
            webSlide = db.As<ICmsSlide>();
            appLog = db.As<IAppLog>();

            ViewBag.AllSlides = webSlide.GetAll();
        }
        
        #region Slides
        public ActionResult Index()
        {
            return View(webSlide.GetAll());
        }

        public ActionResult Record(Guid? Id)
        {
            var model = new CmsSlide();
            if (Id.HasValue)
            {
                model = webSlide.GetById(Id.Value);
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Record(CmsSlide model, HttpPostedFileBase file)
        {
            try
            {
                var slidesPath = Server.MapPath("~/Content/Uploads/Slides/");
                if (file.HasValue())
                {
                    model.Extension = file.FileExtension();
                }
                if (model.Id.IsEmpty())
                {
                    model.Id = webSlide.Create(model);
                    file.SaveAs(slidesPath + model.FileName);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebSlide", "New slide created", "~/CMS/WebSlide/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>New slides created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateCmsSlides("Slide has been created successfully.");

                    TempData["SuccessMsg"] = "Slide has been created successfully.";
                }
                else
                {
                    if (file.HasValue())
                    {
                        file.SaveAs(slidesPath + model.FileName);
                    }
                    webSlide.Update(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebSlide", "Slide updated", "~/CMS/WebSlide/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Slide updated by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    realtime.UpdateCmsSlides("Slide has been updated successfully.");

                    TempData["SuccessMsg"] = "Slide has been updated successfully.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebSlide", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebSlide/Record > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Delete(Guid Id)
        {
            try
            {
                var slidesPath = Server.MapPath("~/Content/Uploads/Slides/");
                var slide = webSlide.GetById(Id);
                if (System.IO.File.Exists(slidesPath + slide.FileName))
                {
                    System.IO.File.Delete(slidesPath + slide.FileName);
                }
                webSlide.Delete(Id);
                

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebSlide", "Slide deleted", "~/CMS/WebSlide/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Slide deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsSlides("Slide has been deleted successfully.");

                TempData["SuccessMsg"] = "Slide has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebSlide", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebSlide/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                        var slidesPath = Server.MapPath("~/Content/Uploads/Slides/");
                        var slide = webSlide.GetById(new Guid(x));
                        if (System.IO.File.Exists(slidesPath + slide.FileName))
                        {
                            System.IO.File.Delete(slidesPath + slide.FileName);
                        }
                        webSlide.Delete(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebSlide", "Multiple slides deleted", "~/CMS/WebSlide/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple slides deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsSlides("Multiple slides has been deleted.");

                TempData["SuccessMsg"] = "Selected slides has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebSlide", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebSlide/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult ChangeSlideStatus(Guid Id)
        {
            try
            {
                webSlide.ChangeStatus(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - WebSlide", "Slide's status changed", "~/CMS/WebSlide/ChangeSlideStatus > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Slides' status changed by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsSlides("Status of the slide has been changed successfully.");

                TempData["SuccessMsg"] = "Status of the slide has been changed successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - WebSlide", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/WebSlide/ChangeSlideStatus > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Json Requests
        public JsonResult GetAllSlides()
        {
            var slides = webSlide.GetAll();
            return Json(slides, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}