using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using CMS.Logic;
using CMS.Entity;
using WebApp.Hubs;

namespace WebApp.Areas.CMS.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All, AppPermission.ViewCMS, AppPermission.CMS)]
    public class FileManagerController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private ICmsFile webFile;
        private IAppLog appLog;

        public FileManagerController()
        {
            webFile = db.As<ICmsFile>();
            appLog = db.As<IAppLog>();
        }

        #region File Manager
        public ActionResult Index()
        {
            return View(webFile.GetAll());
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(HttpPostedFileBase[] files)
        {
            try
            {
                var filesPath = Server.MapPath("~/Content/Uploads/Files/");
                foreach (var file in files)
                {
                    if (file.HasValue())
                    {
                        var cmsFile = new CmsFile
                        {
                            Name = System.IO.Path.GetFileNameWithoutExtension(file.FileName),
                            ContentType = file.ContentType,
                            Size = file.ContentLength,
                            Extension = file.FileExtension()
                        };
                        cmsFile.Id = webFile.Create(cmsFile);
                        file.SaveAs(filesPath + cmsFile.FileName);
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - File Manager", "Files created", "~/CMS/FileManager/Index > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Files created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsFileManager("New files has been created in file manager.");

                TempData["SuccessMsg"] = "Files has been created successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - File Manager", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/FileManager/Index > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                var cmsFilesPath = Server.MapPath("~/Content/Uploads/Files/");
                var cmsFile = webFile.GetById(Id);
                if (System.IO.File.Exists(cmsFilesPath + cmsFile.FileName))
                {
                    System.IO.File.Delete(cmsFilesPath + cmsFile.FileName);
                }
                webFile.Delete(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - File Manager", "File deleted", "~/CMS/FileManager/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>File deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsFileManager("File has been deleted from file manager.");

                TempData["SuccessMsg"] = "File has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - File Manager", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/FileManager/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                        var cmsFilesPath = Server.MapPath("~/Content/Uploads/Files/");
                        var cmsFile = webFile.GetById(new Guid(x));
                        if (System.IO.File.Exists(cmsFilesPath + cmsFile.FileName))
                        {
                            System.IO.File.Delete(cmsFilesPath + cmsFile.FileName);
                        }
                        webFile.Delete(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - File Manager", "Multiple files deleted", "~/CMS/FileManager/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple files deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsFileManager("Multiple files has been deleted from file manager.");

                TempData["SuccessMsg"] = "Selected files has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - File Manager", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/FileManager/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Json Requests
        public JsonResult GetAllFiles()
        {
            var model = webFile.GetAll();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}