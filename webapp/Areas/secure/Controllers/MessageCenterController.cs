using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using System.ComponentModel.DataAnnotations;
using WebApp.Hubs;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize]
    public class MessageCenterController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppMessage appMsg;
        private IAppUser appUser;
        private IAppLog appLog;
        private IAppNotification notify;

        public MessageCenterController()
        {
            appMsg = db.As<IAppMessage>();
            appUser = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            notify = db.As<IAppNotification>();

            ViewBag.AllUsers = appUser.ForceGetAll();
        }

        public ActionResult New(Guid? Id)
        {
            var model = new AppMessage();
            if (Id.HasValue)
            {
                model = appMsg.GetById(Id.Value);
            }
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult New(AppMessage model, HttpPostedFileBase[] attachments, string submit)
        {
            var filesPath = Server.MapPath("~/Content/Uploads/Message/");

            string emailMsgBody = "<table width='100%' cellpadding='10' border='1'>";
            emailMsgBody += "<tr><td width='200px'><strong>Subject</strong></td><td>" + model.Subject + "</td></tr>";
            emailMsgBody += "<tr><td width='200px'><strong>Message</strong></td><td>" + model.Message + "</td></tr>";
            emailMsgBody += "</table>";

            try
            {
                Guid guidOutput;
                if (!string.IsNullOrEmpty(model.Recipients))
                {
                    var validContacts = model.RecipientsList.Where(x => Guid.TryParse(x, out guidOutput)).ToArray();
                    model.Recipients = string.Join(",", validContacts);
                }

                #region Activity Log Description
                var logDesc = "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'>";
                logDesc += "<tr><th class='text-center'>Description</th></tr>";
                logDesc += "<tr><td>Message created by <strong>" + CurrentUser.FullName + "</strong>.</td></tr>";
                logDesc += "<tr><th class='text-center'>Message</th></tr><tr><td>" + model.Message + "</td></tr>";
                if (!string.IsNullOrEmpty(model.Recipients))
                {
                    logDesc += "<tr><th class='text-center'>Internal Recipients</th></tr>";
                    foreach (var x in model.RecipientsList)
                    {
                        var cntact = appUser.GetUserById(new Guid(x));
                        logDesc += "<tr><td>" + cntact.FullName + " (" + cntact.Email + ") </td></tr>";
                    }
                }
                if (!string.IsNullOrEmpty(model.EmailRecipients))
                {
                    logDesc += "<tr><th class='text-center'>External Recipients</th></tr>";
                    foreach (var x in model.EmailRecipientsList)
                    {
                        if (new EmailAddressAttribute().IsValid(x))
                        {
                            logDesc += "<tr><td>" + x + "</td></tr>";
                        }
                        else
                        {
                            logDesc += "<tr><td>Message not sent to <strong>" + x + "</strong> because it is an invalid email.</td></tr>";
                        }
                    }
                }
                logDesc += "</table>";
                #endregion

                if (model.Id.IsEmpty())
                {
                    if (!string.IsNullOrEmpty(model.Recipients) || !string.IsNullOrEmpty(model.EmailRecipients))
                    {
                        #region Create Record
                        if (!string.IsNullOrEmpty(submit))
                        {
                            if (submit == "draft")
                            {
                                model.Status = AppMessageStatus.Draft;
                                TempData["SuccessMsg"] = "Message has been saved as draft successfully.";
                                realtime.UpdateMessages(null);
                            }
                            else
                            {
                                model.Status = AppMessageStatus.Outbox;
                                TempData["SuccessMsg"] = "Message has been sent successfully.";
                                realtime.UpdateMessages(null);
                            }
                        }
                        else
                        {
                            model.Status = AppMessageStatus.Draft;
                            TempData["SuccessMsg"] = "Message has been saved as draft successfully.";
                            realtime.UpdateMessages(null);
                        }
                        model.CreatorId = model.ContactId = CurrentUser.Id;
                        model.Id = appMsg.Create(model);

                        #region Add Attachments
                        foreach (var file in attachments)
                        {
                            if (file.HasValue())
                            {
                                var attachmentId = appMsg.CreateAttachment(model.Id, System.IO.Path.GetFileNameWithoutExtension(file.FileName), file.ContentType, file.FileExtension());
                                file.SaveAs(filesPath + attachmentId);
                            }
                        }
                        #endregion

                        #endregion

                        #region Activity Log
                        if (!string.IsNullOrEmpty(submit))
                        {
                            if (submit == "draft")
                            {
                                appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Message Center", "Message saved as draft", "~/Secure/MessageCenter/New > HttpPost", logDesc);
                                return RedirectToAction("Draft");
                            }
                            else
                            {
                                appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Message Center", "New message sent", "~/Secure/MessageCenter/New > HttpPost", logDesc);
                            }
                        }
                        else
                        {
                            appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Message Center", "Message saved as draft", "~/Secure/MessageCenter/New > HttpPost", logDesc);
                            return RedirectToAction("Draft");
                        }
                        #endregion
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "No message created as there was no recipient selected.";
                        return View(model);
                    }
                }
                else
                {
                    #region Update Record
                    model.Status = AppMessageStatus.Outbox;
                    model.CreatorId = model.ContactId = CurrentUser.Id;
                    appMsg.Update(model);

                    realtime.UpdateMessages(null);
                    #endregion

                    #region Activity Log
                    appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Message Center", "Draft message sent", "~/Secure/MessageCenter/New > HttpPost", logDesc);
                    #endregion

                    TempData["SuccessMsg"] = "Message has been sent successfully.";
                }


                if (!string.IsNullOrEmpty(submit))
                {
                    if (submit == "send")
                    {
                        #region Add System Recipients
                        if (!string.IsNullOrEmpty(model.Recipients))
                        {
                            foreach (var x in model.RecipientsList)
                            {
                                var cntact = appUser.GetUserById(new Guid(x));
                                appMsg.Create(null, model.Id, model.CreatorId, null, AppMessageStatus.Inbox, model.Subject, cntact.Id, null);
                                Emailer.Send(cntact.Email, emailMsgBody, attachments, "You have a new message from " + CurrentUser.FullName + ".");
                            }
                        }
                        #endregion

                        #region Add Email Recipients
                        if (!string.IsNullOrEmpty(model.EmailRecipients))
                        {
                            foreach (var x in model.EmailRecipientsList)
                            {
                                if (new EmailAddressAttribute().IsValid(x))
                                {
                                    emailMsgBody += "<br/><p>You can view and reply to the message by <a href='" + AppSettings.GetVal("notification:URL") + "/Secure/MessageCenter/Thread/" + model.Id + "?e=" + x + "' target='_blank'>clicking here</a>.</p>";
                                    Emailer.Send(x, emailMsgBody, attachments, "You have a new message from " + CurrentUser.FullName + ".");
                                }
                            }
                        }
                        #endregion

                        realtime.UpdateMessages(null);
                    }
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Message Center", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/MessageCenter/New > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Outbox");
        }

        #region Thread
        [AllowAnonymous]
        public ActionResult Thread(Guid Id, string e)
        {
            var model = appMsg.GetById(Id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }

            model.Attachments = appMsg.GetAttachmentsByMessageId(model.Id);
            model.Children = appMsg.GetByParentId(Id).OrderBy(x => x.CreatedOn).ToList();
            foreach (var x in model.Children)
            {
                if (x.ContactId.HasValue)
                {
                    x.Contact = appUser.GetUserById(x.ContactId.Value);
                }
                x.Attachments = appMsg.GetAttachmentsByMessageId(x.Id);
            }
            if (model.ContactId.HasValue)
            {
                model.Contact = appUser.GetUserById(model.ContactId.Value);
                TempData["AuthMsg"] = "You are authorized to view this page.";
            }

            if (Request.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(model.Recipients))
                {
                    if (model.RecipientsList.Any(x => x == CurrentUser.Id.ToString()))
                    {
                        TempData["AuthMsg"] = "You are authorized to view this page.";
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(model.EmailRecipients))
                {
                    if (model.EmailRecipientsList.Any(x => x == e))
                    {
                        TempData["AuthMsg"] = "You are authorized to view this page.";
                    }
                }
            }
            return View(model);
        }
        [HttpPost, AllowAnonymous]
        public ActionResult Thread(AppMessage model, string e, HttpPostedFileBase[] attachments)
        {
            var filesPath = Server.MapPath("~/Content/Uploads/Message/");

            try
            {
                var parentMsg = appMsg.GetById(model.ParentId.Value);

                string emailMsgBody = "<table width='100%' cellpadding='10' border='1'>";
                emailMsgBody += "<tr><td width='200px'><strong>Subject</strong></td><td>" + parentMsg.Subject + "</td></tr>";
                emailMsgBody += "<tr><td width='200px'><strong>Message</strong></td><td>" + model.Message + "</td></tr>";
                emailMsgBody += "</table>";

                if (!string.IsNullOrEmpty(e))
                {
                    if (parentMsg.EmailRecipientsList.Any(x => x == e))
                    {
                        #region Create Message
                        model.CreatorEmail = model.Email = e;
                        model.ParentId = parentMsg.Id;
                        model.Subject = parentMsg.Subject;
                        model.Status = AppMessageStatus.Outbox;
                        model.Id = appMsg.Create(model);

                        #region Add Attachments
                        foreach (var file in attachments)
                        {
                            if (file.HasValue())
                            {
                                var attachmentId = appMsg.CreateAttachment(model.Id, System.IO.Path.GetFileNameWithoutExtension(file.FileName), file.ContentType, file.FileExtension());
                                file.SaveAs(filesPath + attachmentId);
                            }
                        }
                        #endregion
                        
                        #endregion

                        #region Send to parent as sender as well
                        var prntcontact = appUser.GetUserById(parentMsg.ContactId.Value);
                        appMsg.Create(null, parentMsg.Id, null, model.CreatorEmail, AppMessageStatus.Inbox, parentMsg.Subject, prntcontact.Id, null);
                        Emailer.Send(prntcontact.Email, emailMsgBody, attachments, "You have a new message from " + e + ".");
                        #endregion

                        #region Add System Recipients
                        if (!string.IsNullOrEmpty(parentMsg.Recipients))
                        {
                            foreach (var x in parentMsg.RecipientsList)
                            {
                                #region Send Email
                                var cntact = appUser.GetUserById(new Guid(x));
                                appMsg.Create(null, parentMsg.Id, null, model.CreatorEmail, AppMessageStatus.Inbox, parentMsg.Subject, cntact.Id, null);
                                Emailer.Send(cntact.Email, emailMsgBody, attachments, "You have a new message from " + e + ".");
                                #endregion
                            }
                        }
                        #endregion

                        #region Add Email Recipients
                        if (!string.IsNullOrEmpty(parentMsg.EmailRecipients))
                        {
                            foreach (var x in parentMsg.EmailRecipientsList)
                            {
                                if (new EmailAddressAttribute().IsValid(x) && x != e)
                                {
                                    emailMsgBody += "<br/><p>You can view and reply to the message by <a href='" + AppSettings.GetVal("notification:URL") + "/Secure/MessageCenter/Thread/" + model.ParentId + "?e=" + x + "' target='_blank'>clicking here</a>.</p>";
                                    Emailer.Send(x, emailMsgBody, attachments, "You have a new message from " + e + ".");
                                }
                            }
                        }
                        #endregion

                        realtime.UpdateMessages(e + " replied to a message.");

                        TempData["SuccessMsg"] = "Your message has been sent successfully.";
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "You are not authorized to reply to this message.";
                    }
                    return RedirectToAction("Thread", new { Id = model.ParentId, e = e });
                }
                else
                {
                    if (CurrentUser.Id == parentMsg.ContactId)
                    {
                        #region Create Message
                        model.CreatorId = model.ContactId = CurrentUser.Id;
                        model.ParentId = parentMsg.Id;
                        model.Subject = parentMsg.Subject;
                        model.Status = AppMessageStatus.Outbox;
                        model.Id = appMsg.Create(model);

                        #region Add Attachments
                        foreach (var file in attachments)
                        {
                            if (file.HasValue())
                            {
                                var attachmentId = appMsg.CreateAttachment(model.Id, System.IO.Path.GetFileNameWithoutExtension(file.FileName), file.ContentType, file.FileExtension());
                                file.SaveAs(filesPath + attachmentId);
                            }
                        }
                        #endregion

                        #endregion

                        #region Add System Recipients
                        if (!string.IsNullOrEmpty(parentMsg.Recipients))
                        {
                            foreach (var x in parentMsg.RecipientsList)
                            {
                                #region Send Email
                                var cntact = appUser.GetUserById(new Guid(x));
                                appMsg.Create(null, parentMsg.Id, model.CreatorId, null, AppMessageStatus.Inbox, parentMsg.Subject, cntact.Id, null);
                                Emailer.Send(cntact.Email, emailMsgBody, attachments, "You have a new message from " + CurrentUser.FullName + ".");
                                #endregion
                            }
                        }
                        #endregion

                        #region Add Email Recipients
                        if (!string.IsNullOrEmpty(parentMsg.EmailRecipients))
                        {
                            foreach (var x in parentMsg.EmailRecipientsList)
                            {
                                if (new EmailAddressAttribute().IsValid(x))
                                {
                                    emailMsgBody += "<br/><p>You can view and reply to the message by <a href='" + AppSettings.GetVal("notification:URL") + "/Secure/MessageCenter/Thread/" + model.ParentId + "?e=" + x + "' target='_blank'>clicking here</a>.</p>";
                                    Emailer.Send(x, emailMsgBody, attachments, "You have a new message from " + CurrentUser.FullName + ".");
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        if (parentMsg.RecipientsList.Any(x => x == CurrentUser.Id.ToString()))
                        {
                            #region Create Message
                            model.CreatorId = model.ContactId = CurrentUser.Id;
                            model.ParentId = parentMsg.Id;
                            model.Subject = parentMsg.Subject;
                            model.Status = AppMessageStatus.Outbox;
                            model.Id = appMsg.Create(model);

                            #region Add Attachments
                            foreach (var file in attachments)
                            {
                                if (file.HasValue())
                                {
                                    var attachmentId = appMsg.CreateAttachment(model.Id, System.IO.Path.GetFileNameWithoutExtension(file.FileName), file.ContentType, file.FileExtension());
                                    file.SaveAs(filesPath + attachmentId);
                                }
                            }
                            #endregion

                            #region Send to parent as sender as well
                            var prntcontact = appUser.GetUserById(parentMsg.ContactId.Value);
                            appMsg.Create(null, parentMsg.Id, model.CreatorId, null, AppMessageStatus.Inbox, parentMsg.Subject, prntcontact.Id, null);
                            Emailer.Send(prntcontact.Email, emailMsgBody, attachments, "You have a new message from " + CurrentUser.FullName + ".");
                            #endregion

                            #region Add System Recipients
                            if (string.IsNullOrEmpty(parentMsg.Recipients))
                            {
                                foreach (var x in parentMsg.RecipientsList)
                                {
                                    if (CurrentUser.Id != new Guid(x))
                                    {
                                        #region Send Email
                                        var cntact = appUser.GetUserById(new Guid(x));
                                        appMsg.Create(null, parentMsg.Id, model.CreatorId, null, AppMessageStatus.Inbox, parentMsg.Subject, cntact.Id, null);
                                        Emailer.Send(cntact.Email, emailMsgBody, attachments, "You have a new message from " + CurrentUser.FullName + ".");
                                        #endregion
                                    }
                                }
                            }
                            #endregion

                            #region Add Email Recipients
                            if (!string.IsNullOrEmpty(parentMsg.EmailRecipients))
                            {
                                foreach (var x in parentMsg.EmailRecipientsList)
                                {
                                    if (new EmailAddressAttribute().IsValid(x))
                                    {
                                        emailMsgBody += "<br/><p>You can view and reply to the message by <a href='" + AppSettings.GetVal("notification:URL") + "/Secure/MessageCenter/Thread/" + model.ParentId + "?e=" + x + "' target='_blank'>clicking here</a>.</p>";
                                        Emailer.Send(x, emailMsgBody, attachments, "You have a new message from " + CurrentUser.FullName + ".");
                                    }
                                }
                            }
                            #endregion

                            #endregion

                            #region Activity Log
                            appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Message Center", "Replied to the message", "~/Secure/MessageCenter/Thread > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Replied to the message by <strong>" + CurrentUser.FullName + "</strong>.</td></tr><tr><th class='text-center'>Message</th></tr><tr><td>" + model.Message + "</td></tr></table>");
                            #endregion

                            TempData["SuccessMsg"] = "Your message has been sent successfully.";
                        }
                        else
                        {
                            TempData["ErrorMsg"] = "You are not authorized to reply to this message.";
                        }
                    }
                    realtime.UpdateMessages(CurrentUser.FullName + " replied to a message.");
                }
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Message Center", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/MessageCenter/Thread > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return RedirectToAction("Thread", new { Id = model.ParentId });
        }
        #endregion

        public ActionResult Index()
        {
            var model = appMsg.GetByContactIdAndStatus(CurrentUser.Id, AppMessageStatus.Inbox);
            foreach (var x in model)
            {
                if (x.ParentId.HasValue)
                {
                    x.Parent = appMsg.GetById(x.ParentId.Value);
                    if (x.Parent.ContactId.HasValue)
                    {
                        x.Parent.Contact = appUser.GetUserById(x.Parent.ContactId.Value);
                    }
                }
                if (x.ContactId.HasValue)
                {
                    x.Contact = appUser.GetUserById(x.ContactId.Value);
                }
                if (x.CreatorId.HasValue)
                {
                    x.Creator = appUser.GetUserById(x.CreatorId.Value);
                }
            }
            return View(model);
        }

        public ActionResult Outbox()
        {
            var model = appMsg.GetByContactIdAndStatus(CurrentUser.Id, AppMessageStatus.Outbox);
            foreach (var x in model)
            {
                if (x.CreatorId.HasValue)
                {
                    x.Creator = appUser.GetUserById(x.CreatorId.Value);
                }
                if (x.ContactId.HasValue)
                {
                    x.Contact = appUser.GetUserById(x.ContactId.Value);
                }
                if (x.ParentId.HasValue)
                {
                    x.Parent = appMsg.GetById(x.ParentId.Value);
                    if (x.Parent.CreatorId.HasValue)
                    {
                        x.Parent.Creator = appUser.GetUserById(x.Parent.CreatorId.Value);
                    }
                    if (x.Parent.ContactId.HasValue)
                    {
                        x.Parent.Contact = appUser.GetUserById(x.Parent.ContactId.Value);
                    }
                }
            }
            return View(model);
        }

        public ActionResult Draft()
        {
            var model = appMsg.GetByContactIdAndStatus(CurrentUser.Id, AppMessageStatus.Draft);
            foreach (var x in model)
            {
                if (x.CreatorId.HasValue)
                {
                    x.Creator = appUser.GetUserById(x.CreatorId.Value);
                }
                if (x.ContactId.HasValue)
                {
                    x.Contact = appUser.GetUserById(x.ContactId.Value);
                }
                if (x.ParentId.HasValue)
                {
                    x.Parent = appMsg.GetById(x.ParentId.Value);
                    if (x.Parent.CreatorId.HasValue)
                    {
                        x.Parent.Creator = appUser.GetUserById(x.Parent.CreatorId.Value);
                    }
                    if (x.Parent.ContactId.HasValue)
                    {
                        x.Parent.Contact = appUser.GetUserById(x.Parent.ContactId.Value);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult Delete(Guid Id)
        {
            try
            {
                var attachmentsPath = Server.MapPath("~/Content/Uploads/Message/");
                var model = appMsg.GetById(Id);
                model.Attachments = appMsg.GetAttachmentsByMessageId(Id);

                foreach (var x in model.Attachments)
                {
                    if (System.IO.File.Exists(attachmentsPath + x.Id))
                    {
                        System.IO.File.Delete(attachmentsPath + x.Id);
                    }
                }

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Message Center", "Message deleted", "~/Secure/MessageCenter/Delete > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Message was deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr><tr><th class='text-center'>Message</th></tr><tr><td>" + model.Message + "</td></tr></table>");
                #endregion

                appMsg.Delete(Id);
                appMsg.DeleteAttachmentsByMessageId(Id);
                appMsg.DeleteByParentId(Id);
                realtime.UpdateMessages("Message deleted.");

                TempData["SuccessMsg"] = "Message has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Message Center", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/MessageCenter/Delete > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                var attachmentsPath = Server.MapPath("~/Content/Uploads/Message/");

                var idsList = Ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(Ids))
                {
                    foreach (var x in idsList)
                    {
                        var model = appMsg.GetById(new Guid(x));
                        model.Attachments = appMsg.GetAttachmentsByMessageId(model.Id);

                        foreach (var xx in model.Attachments)
                        {
                            if (System.IO.File.Exists(attachmentsPath + xx.Id))
                            {
                                System.IO.File.Delete(attachmentsPath + xx.Id);
                            }
                        }

                        #region Activity Log
                        appLog.Create(CurrentUser.OfficeId, model.Id, CurrentUser.Id, AppLogType.Activity, "Message Center", "Message deleted", "~/Secure/MessageCenter/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Message was deleted by <strong>" + CurrentUser.FullName + "</strong>.</td></tr><tr><th class='text-center'>Message</th></tr><tr><td>" + model.Message + "</td></tr></table>");
                        #endregion

                        appMsg.Delete(model.Id);
                        appMsg.DeleteAttachmentsByMessageId(model.Id);
                        appMsg.DeleteByParentId(model.Id);

                        realtime.UpdateMessages("Multiple messages deleted.");
                    }
                }

                TempData["SuccessMsg"] = "Selected messages has been deleted successfully.";
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Message Center", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/MessageCenter/DeleteMultiple > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }

            return Json(true);
        }

        #region Remote Functions
        public PartialViewResult Reader()
        {
            var model = appMsg.GetByContactIdAndStatus(CurrentUser.Id, AppMessageStatus.Inbox);
            foreach (var x in model)
            {
                if (x.CreatorId.HasValue)
                {
                    x.Creator = appUser.GetUserById(x.CreatorId.Value);
                }
                if (x.ContactId.HasValue)
                {
                    x.Contact = appUser.GetUserById(x.ContactId.Value);
                }
                if (x.ParentId.HasValue)
                {
                    x.Parent = appMsg.GetById(x.ParentId.Value);
                }
            }
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult MarkReadAll()
        {
            try
            {
                appMsg.MarkAllRead(CurrentUser.Id);
                realtime.UpdateMessages("All messages marked as read.");

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Activity, "Message Center", "Marked all messages as read", "~/Secure/MessageCenter/MarkAllRead > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Marked all messages as read by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Message Center", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/MessageCenter/MarkAllRead > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
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
                appMsg.MarkRead(Id);
                realtime.UpdateMessages("Message marked as read.");

                #region Activity Log
                appLog.Create(CurrentUser.OfficeId, Id, CurrentUser.Id, AppLogType.Activity, "Message Center", "Message marked as read", "~/Secure/MessageCenter/MarkRead > HttpPost", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Message marked as read by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Log
                appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Message Center", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/MessageCenter/MarkRead > HttpPost", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                #endregion

                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
            }
            return Json(true);
        }

        [AllowAnonymous]
        public ActionResult Attachment(Guid Id)
        {
            try
            {
                var file = appMsg.GetAttachmentById(Id);
                var filePath = Server.MapPath("~/Content/Uploads/Message/") + Id;
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = file.Title,
                    Inline = true
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                #region Activity Log
                if (User.Identity.IsAuthenticated)
                {
                    appLog.Create(CurrentUser.OfficeId, file.MessageId, CurrentUser.Id, AppLogType.Activity, "Message Center", "Attachment viewed / downloaded", "~/Secure/MessageCenter/Attachment", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Attachment viewed / downloaded by <strong>" + CurrentUser.FullName + "</strong>.</td></tr></table>");
                }
                else
                {
                    appLog.Create(null, file.MessageId, null, AppLogType.Activity, "Message Center", "Attachment viewed / downloaded", "~/Secure/MessageCenter/Attachment", "<table class='table table-hover table-striped table-condensed' style='margin-bottom:15px;'><tr><th class='text-center'>Description</th></tr><tr><td>Attachment viewed / downloaded by <strong>" + Request.UserHostAddress + "</strong>.</td></tr></table>");
                }
                #endregion

                return File(filePath, file.Type);
            }
            catch (Exception ex)
            {
                #region Error Log
                if (User.Identity.IsAuthenticated)
                {
                    appLog.Create(CurrentUser.OfficeId, null, CurrentUser.Id, AppLogType.Error, "Message Center", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/MessageCenter/Attachment", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                }
                else
                {
                    appLog.Create(null, null, null, AppLogType.Error, "Message Center", ex.GetType().Name.ToSpacedTitleCase(), "~/Secure/MessageCenter/Attachment", "<table class='table table-hover table-striped'><tr><th class='text-right'>Source</th><td>" + ex.Source + "</td></tr><tr><th class='text-right'>URL</th><td>" + Request.Url.ToString() + "</td></tr><tr><th class='text-right'>Message</th><td>" + ex.Message + "</td></tr></table><table class='table table-hover table-striped table-condensed'><tr><th class='text-center'>Inner Exception</th></tr><tr><td>" + ex.InnerException + "</td></tr><tr><th class='text-center'>Stack Trace</th></tr><tr><td>" + ex.StackTrace.ToString() + "</td></tr></table>");
                }
                TempData["ErrorMsg"] = "We have encountered an error while processing your request, Please see log for details.";
                #endregion

                return RedirectToAction(Url.Action("Error", "AppLog", new { Area = "Secure" }));
            }
        }
        #endregion
    }
}