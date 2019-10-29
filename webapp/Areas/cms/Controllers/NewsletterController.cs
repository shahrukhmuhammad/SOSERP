using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using CMS.Logic;
using CMS.Entity;
using System.ComponentModel.DataAnnotations;
using WebApp.Hubs;

namespace WebApp.Areas.CMS.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All, AppPermission.ViewCMS, AppPermission.CMS)]
    public class NewsletterController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private ICmsNewsletter newsLetter;
        private IAppLog appLog;

        public NewsletterController()
        {
            newsLetter = db.As<ICmsNewsletter>();
            appLog = db.As<IAppLog>();
        }

        #region Newsletter
        public ActionResult Index()
        {
            return View(newsLetter.GetAll());
        }

        public ActionResult Record(Guid? Id)
        {
            var model = new CmsNewsletter();
            if (Id.HasValue)
            {
                model = newsLetter.GetById(Id.Value);
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Record(CmsNewsletter model, string submit)
        {
            try
            {
                if (model.Id.IsEmpty())
                {
                    if (!string.IsNullOrEmpty(submit))
                    {
                        if (submit == "Send")
                        {
                            model.Status = CmsNewsletterStatus.Sent;
                            TempData["SuccessMsg"] = "Newsletter has been created and sent successfully.";
                        }
                        else
                        {
                            model.Status = CmsNewsletterStatus.Draft;
                            TempData["SuccessMsg"] = "Newsletter has been saved as draft successfully.";
                        }
                    }
                    else
                    {
                        model.Status = CmsNewsletterStatus.Draft;
                        TempData["SuccessMsg"] = "Newsletter has been saved as draft successfully.";
                    }

                    string allRecipients = "";
                    foreach (var x in newsLetter.GetAllEmails())
                    {
                        allRecipients = string.Join(",", x.Email);
                    }
                    model.Recipients = allRecipients;
                    newsLetter.Create(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Newsletter", "Newsletter created", "~/CMS/Newsletter/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Newsletter created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion
                }
                else
                {
                    model.Status = CmsNewsletterStatus.Sent;
                    string allRecipients = "";
                    foreach (var x in newsLetter.GetAllEmails())
                    {
                        allRecipients = string.Join(",", x.Email);
                    }
                    model.Recipients = allRecipients;
                    newsLetter.Update(model);

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Newsletter", "Draft newsletter sent", "~/CMS/Newsletter/Record > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Draft newsletter sent by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                    #endregion

                    TempData["SuccessMsg"] = "Draft newsletter has been sent successfully.";
                }

                if (!string.IsNullOrEmpty(submit))
                {
                    if (submit == "Send")
                    {
                        foreach (var x in newsLetter.GetAllEmails())
                        {
                            Emailer.Send(x.Email, model.Contents, model.Subject);
                        }
                    }
                }

                realtime.UpdateCmsNewsletterPosts("Newsletter posts updated.");
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - Newsletter", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/Record/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                newsLetter.Delete(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Newsletter", "Newsletter deleted", "~/CMS/Newsletter/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Newsletter deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsNewsletterPosts("Newsletter post has been deleted.");

                TempData["SuccessMsg"] = "Newsletter has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - Newsletter", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/Newsletter/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                        newsLetter.Delete(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Newsletter", "Multiple newsletter deleted", "~/CMS/Newsletter/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple newsletter deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsNewsletterPosts("Multiple newsletter posts has been deleted.");

                TempData["SuccessMsg"] = "Selected newsletter has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - Newsletter", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/Newsletter/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Subscribers
        public ActionResult Subscribers()
        {
            return View(newsLetter.GetAllEmails());
        }
        [HttpPost]
        public ActionResult Subscribers(string Email)
        {
            try
            {
                if (!string.IsNullOrEmpty(Email))
                {
                    if (new EmailAddressAttribute().IsValid(Email))
                    {
                        newsLetter.CreateEmail(Email);

                        #region Activity Log
                        appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Newsletter", "Subscriber added", "~/CMS/Newsletter/Subscribers > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Subscriber added by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                        #endregion

                        realtime.UpdateCmsNewsletterSubscribers("Newsletter email has been added successfully.");

                        TempData["SuccessMsg"] = "Newsletter email has been added successfully.";
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "Provided email address is not valid.";
                    }
                }
                else
                {
                    TempData["ErrorMsg"] = "Email not provided.";
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - Newsletter", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/Newsletter/Subscribers > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Subscribers");
        }

        [HttpPost]
        public JsonResult CheckDuplicateEmail(string Email)
        {
            if (newsLetter.CheckDuplicateEmail(Guid.Empty, Email))
            {
                return Json(true);
            }
            else
            {
                return Json("Email already exists.");
            }
        }

        [HttpPost]
        public JsonResult DeleteEmail(Guid Id)
        {
            try
            {
                newsLetter.DeleteEmailById(Id);

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Newsletter", "Email deleted", "~/CMS/Newsletter/DeleteEmail > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Email deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsNewsletterSubscribers("Newsletter email has been deleted.");

                TempData["SuccessMsg"] = "Newsletter email has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - Newsletter", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/Newsletter/DeleteEmail > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [HttpPost]
        public JsonResult DeleteMultipleEmails(string Ids)
        {
            try
            {
                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        newsLetter.DeleteEmailById(new Guid(x));
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Cms - Newsletter", "Multiple emails deleted", "~/CMS/Newsletter/DeleteMultipleEmails > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Multiple emails deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion

                realtime.UpdateCmsNewsletterSubscribers("Multiple newsletter emails has been deleted.");

                TempData["SuccessMsg"] = "Selected emails has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Cms - Newsletter", ex.GetType().Name.ToSpacedTitleCase(), "~/CMS/Newsletter/DeleteMultipleEmails > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }
        #endregion

        #region Json Requests
        public JsonResult GetAllSubscribers()
        {
            var subscribers = newsLetter.GetAllEmails();
            return Json(subscribers, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllPosts()
        {
            var posts = newsLetter.GetAll();
            return Json(posts, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}