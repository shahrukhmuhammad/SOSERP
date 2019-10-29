using BaseApp.Entity;
using BaseApp.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using DMS.Logic;
using DMS.System;
using DMS.Entity;
using ImageResizer;
using WebApp.Hubs;

namespace WebApp.Areas.DMS.Controllers
{
    [ModuleActivator, AppAuthorize]
    public class DocsController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        public ActionResult QuickUpload()
        {
            return View();
        }

        public ActionResult All()
        {
            return View();
        }

        public ActionResult Personal()
        {
            return View();
        }

        public ActionResult Shared()
        {
            return View();
        }

        public ActionResult Index(Guid id)
        {
            try
            {
                var dmsRepo = db.As<DmsRepository>();
                return View(dmsRepo.SelectById(id));
            }
            catch
            {
                return Redirect(BaseUrl.GetBaseUrl());
            }
        }

        public ActionResult SharedDocs()
        {
            try
            {
                var dmsRepo = db.As<DmsRepository>();
                return View(dmsRepo.GetSharedContentsByUserId(CurrentUser.Id));
            }
            catch
            {
                return Redirect(BaseUrl.GetBaseUrl());
            }
        }

        public ActionResult RenderDms(string path, string viewMode)
        {
            var dmsRepo = db.As<DmsRepository>();
            return Content(dmsRepo.RenderPath(path, viewMode));
        }

        public ActionResult RenderShared()
        {
            var dmsRepo = db.As<DmsRepository>();
            return Content(dmsRepo.RenderShared());
        }

        public string ParsePath(string path)
        {
            var dmsRepo = db.As<DmsRepository>();
            return dmsRepo.GetRouteId(path);
        }

        public ActionResult Search(Guid id, string q)
        {
            var dmsRepo = db.As<DmsRepository>();
            var model = dmsRepo.GetSearchResults(id, q);
            return View(model);
        }

        public ActionResult Browse(Guid id)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[8];
            var random = new Random();
            for (int i = 0; i < stringChars.Length; i++) stringChars[i] = chars[random.Next(chars.Length)];

            return Content(@"<iframe src='/dms/index/" + id + "/?p=" + id + "&mode=" + DmsRenderMode.Browse + "' name='" + new string(stringChars) + "' class='dms-frame' name='dmsFrame' frameborder='0' scrolling='no' onload='resizeIframe(this)'></iframe>");
        }

        public ActionResult V(Guid id)
        {
            var dmsRepo = db.As<DmsRepository>();

            return View(dmsRepo.GetFileById(id));
        }

        public ActionResult DmsItems(IList<DmsItem> model)
        {
            return PartialView("_DmsItems", model);
        }

        public ActionResult Download(string id, string dir)
        {
            var dmsPath = Server.MapPath("~/app_data/dms/");
            var ids = id.Split(',');

            var dmsRepo = db.As<DmsRepository>();

            var files = db.QuerySql<DmsItem>("SELECT Id, Title, Extension, ResourceType, ContentType, ParentId FROM DmsRepository WHERE Id IN (@Id)", new { Id = ids });

            if (files.Count == 1 && files[0].ResourceType != DmsResourceType.Folder)
            {
                return File(dmsPath + id, files[0].ContentType, files[0].Filename);
            }
            else
            {
                Response.Buffer = false;
                return File(dmsRepo.ZipBytes(id), "application/zip", dir == "" ? "dms-download" : dir + ".zip");
            }
        }

        public ActionResult GetContent(Guid id)
        {
            var dmsRepo = db.As<DmsRepository>();
            var file = dmsRepo.GetFileById(id);
            string dmsPath = Server.MapPath("~/app_data/dms/");
            if (System.IO.File.Exists(dmsPath + file.Id))
            {
                return Content(HttpUtility.HtmlEncode(System.IO.File.ReadAllText(dmsPath + file.Id)));
            }
            else return Content("File not found.");
        }

        public ActionResult GetRevisions(Guid id)
        {
            var dmsRepo = db.As<DmsRepository>();
            return PartialView(dmsRepo.GetRevisions(id));
        }

        public ActionResult GetPermissions(Guid id)
        {
            var dmsRepo = db.As<DmsRepository>();
            var getUsers = db.QuerySql<AppUser>("SELECT Id, FirstName, MiddleName, LastName, RoleId FROM AppUsers WHERE Id NOT IN(SELECT UserId FROM DmsRepositoryPermissions WHERE DocId=@Id)", new { Id = id });
            if (getUsers.Count() > 0)
            {
                foreach (var x in getUsers)
                {
                    x.Role = db.QuerySql<AppRole>("SELECT * FROM AppRoles WHERE Id = @Id", new { Id = x.RoleId }).SingleOrDefault();
                }
            }
            ViewBag.Users = getUsers; 
            return PartialView(dmsRepo.GetPermissionsByItemId(id));
        }

        [HttpPost]
        public ActionResult SetPermissions(Guid id, string ids)
        {
            var dmsRepo = db.As<DmsRepository>();
            string[] list = ids.Split(',');

            db.QuerySql("DELETE FROM DmsRepositoryPermissions WHERE DocId=@Id", new { Id = id });
            foreach (var i in list)
            {
                db.QuerySql("INSERT INTO DmsRepositoryPermissions (DocId, UserId) VALUES(@DocId, @UserId)", new { DocId = id, UserId = i });
            }

            return Json(true);
        }

        public ActionResult Restore(Guid id, Guid prnt, DmsRenderMode mode = DmsRenderMode.View)
        {
            var dmsRepo = db.As<DmsRepository>();
            string dmsPath = Server.MapPath("~/app_data/dms/");
            var newItem = dmsRepo.RestoreRevision(id, CurrentUser.Id);
            System.IO.File.Copy(dmsPath + id, dmsPath + newItem.Id);

            realtime.UpdateDMS("File(s) restored in DMS.");

            return Redirect("/DMS/Docs/Index/" + newItem.ParentId + "/?p=" + prnt + "&mode=" + mode);
        }

        [HttpPost]
        public ActionResult drupload(HttpPostedFileBase[] files, Guid parentId)
        {
            var dmsRepo = db.As<DmsRepository>();

            foreach (var file in files)
            {
                if (file.HasValue())
                {
                    var dmsItem = new DmsItem
                    {
                        Title = HttpUtility.UrlDecode(System.IO.Path.GetFileNameWithoutExtension(file.FileName)),
                        ResourceType = DmsResourceType.File,
                        FileSize = file.ContentLength,
                        Extension = file.FileExtension(),
                        ParentId = parentId,
                        CreatedBy = CurrentUser.Id,
                        ContentType = file.ContentType
                    };
                    dmsItem.Id = dmsRepo.Insert(dmsItem);

                    var dmsPath = Server.MapPath("~/app_data/dms/");
                    file.SaveAs(dmsPath + dmsItem.Id);
                }
            }
            return Json(true);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase[] files, Guid parentId, Guid prnt, DmsRenderMode mode = DmsRenderMode.View)
        {
            var dmsRepo = db.As<DmsRepository>();

            foreach (var file in files)
            {
                if (file.HasValue())
                {
                    if (Request.Browser.IsMobileDevice)
                    {
                        if (Request.UserAgent.Contains("iPhone"))
                        {
                            var dmsItem = new DmsItem
                            {
                                Title = DateTime.UtcNow.ToString("ffffff") + "_iOS",
                                ResourceType = DmsResourceType.File,
                                FileSize = file.ContentLength,
                                Extension = file.FileExtension(),
                                ParentId = parentId,
                                CreatedBy = CurrentUser.Id,
                                ContentType = file.ContentType
                            };
                            dmsItem.Id = dmsRepo.Insert(dmsItem);

                            var dmsPath = Server.MapPath("~/app_data/dms/");
                            file.SaveAs(dmsPath + dmsItem.Id);
                        }
                        else
                        {
                            var dmsItem = new DmsItem
                            {
                                Title = DateTime.UtcNow.ToString("ffffff") + "_android",
                                ResourceType = DmsResourceType.File,
                                FileSize = file.ContentLength,
                                Extension = file.FileExtension(),
                                ParentId = parentId,
                                CreatedBy = CurrentUser.Id,
                                ContentType = file.ContentType
                            };
                            dmsItem.Id = dmsRepo.Insert(dmsItem);

                            var dmsPath = Server.MapPath("~/app_data/dms/");
                            file.SaveAs(dmsPath + dmsItem.Id);
                        }
                    }
                    else
                    {
                        var dmsItem = new DmsItem
                        {
                            Title = System.IO.Path.GetFileNameWithoutExtension(file.FileName),
                            ResourceType = DmsResourceType.File,
                            FileSize = file.ContentLength,
                            Extension = file.FileExtension(),
                            ParentId = parentId,
                            CreatedBy = CurrentUser.Id,
                            ContentType = file.ContentType
                        };
                        dmsItem.Id = dmsRepo.Insert(dmsItem);

                        var dmsPath = Server.MapPath("~/app_data/dms/");
                        file.SaveAs(dmsPath + dmsItem.Id);
                    }
                }

            }

            realtime.UpdateDMS("New file(s) added to DMS.");

            return Redirect("/DMS/Docs/Index/" + parentId + "/?p=" + prnt + "&mode=" + mode);
        }

        public ActionResult Delete(string id, Guid parentId, Guid prnt, DmsRenderMode mode = DmsRenderMode.View)
        {
            var dmsRepo = db.As<DmsRepository>();

            var dmsPath = Server.MapPath("~/app_data/dms/");

            string[] ids = id.Split(',');

            List<string> AllFiles = new List<string>();

            var delables = db.QuerySql<DmsItem>("SELECT Id, ResourceType FROM DmsRepository WHERE Id IN(@Id) AND IsSystem=0", new { Id = ids });

            foreach (var i in delables)
            {
                AllFiles.Add(i.Id.ToString());
            }

            foreach (var f in delables.Where(x => x.ResourceType == DmsResourceType.Folder))
            {
                foreach (var m in dmsRepo.GetAllChildIdsByDirId(f.Id))
                {
                    AllFiles.Add(m.Id.ToString());
                }
            }

            var delableFiles = delables.Where(x => x.ResourceType == DmsResourceType.File).Select(x => x.Id).ToArray();
            var revisions = db.QuerySql<DmsItem>("SELECT Id FROM DmsRepository WHERE ParentId=@ParentId AND Title IN(SELECT Title FROM DmsRepository WHERE Id IN(@Id)) AND Extension IN(SELECT Extension FROM DmsRepository WHERE Id IN(@Id)) AND Id NOT IN(@Id)", new { Id = delableFiles, ParentId = parentId });
            foreach (var v in revisions)
            {
                AllFiles.Add(v.Id.ToString());
            }

            db.QuerySql("DELETE FROM DmsRepository WHERE Id IN(@Id); DELETE FROM DmsRepositoryPermissions WHERE DocId IN (@Id)", new { Id = AllFiles.ToArray() });

            foreach (string i in AllFiles)
            {
                if (System.IO.File.Exists(dmsPath + i))
                {
                    System.IO.File.Delete(dmsPath + i);
                }
            }

            realtime.UpdateDMS("File(s) deleted from DMS.");

            return Redirect("/DMS/Docs/Index/" + parentId + "/?p=" + prnt + "&mode=" + mode);
        }

        [HttpPost]
        public ActionResult Title(string title, Guid? itemId, Guid? parentId)
        {
            var dmsRepo = db.As<DmsRepository>();

            if (!itemId.HasValue)
            {
                int dirExists = db.QuerySql<int>("SELECT Count(1) FROM DmsRepository WHERE ParentId=@ParentId AND Title=@Title AND ResourceType=1", new { Title = title, ParentId = parentId }).SingleOrDefault();
                if (dirExists < 1)
                {
                    dmsRepo.Insert(new DmsItem { ParentId = parentId, Title = title, ResourceType = DmsResourceType.Folder, CreatedBy = CurrentUser.Id });
                    realtime.UpdateDMS(title + " folder created in DMS.");
                    return Json(false);
                }
                else
                {
                    return Json(true);
                }
            }
            else
            {
                return Json(dmsRepo.Update(new DmsItem { Title = title, Id = itemId.Value, ParentId = parentId }));
            }
        }

        public ActionResult Breadcrumb(Guid id, Guid p, string mode)
        {
            var dmsRepo = db.As<DmsRepository>();

            var ul = new TagBuilder("ul");
            ul.MergeAttribute("class", "breadcrumb");

            var path = dmsRepo.BreadCrumbByDirId(id);

            foreach (var dir in path)
            {
                ul.InnerHtml = "<li><a href='/dms/docs/index/" + dir.Id + "/?p=" + p + "&mode=" + mode + "' title='" + (dir.Title.Split('-')[0] == "Usr" ? "My Files" : dir.Title) + "'>" + (dir.Title.Split('-')[0] == "Usr" ? "My Files" : dir.Title) + "</a></li>" + ul.InnerHtml;
                if (dir.Id == p) break;
            }

            return Content(ul.ToString());
        }

        public void Get(Guid id)
        {
            var filePath = Server.MapPath("~/app_data/dms/" + id);

            var dmsRepo = db.As<DmsRepository>();

            var file = dmsRepo.GetFileById(id);

            if (System.IO.File.Exists(filePath))
            {
                Response.Clear();
                Response.Buffer = false;
                Response.AddHeader("Content-Disposition", "filename=\"" + file.Filename + "\"; filename*=UTF-8''" + Uri.EscapeDataString(file.Filename));
                Response.ContentType = file.ContentType;

                if (Request.QueryString != null && (file.ContentType == "image/jpeg" || file.ContentType == "image/pjpeg" || file.ContentType == "image/png" || file.ContentType == "image/gif"))
                {
                    byte[] byteArray = new byte[0];
                    using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                    {
                        if (file.Extension == "png")
                        {
                            ImageBuilder.Current.Build(filePath, new ResizeSettings(Request.QueryString)).Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else if (file.Extension == "gif")
                        {
                            ImageBuilder.Current.Build(filePath, new ResizeSettings(Request.QueryString)).Save(stream, System.Drawing.Imaging.ImageFormat.Gif);
                        }
                        else
                        {
                            ImageBuilder.Current.Build(filePath, new ResizeSettings(Request.QueryString)).Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        stream.Close();
                        byteArray = stream.ToArray();
                    }
                    Response.BinaryWrite(byteArray);
                }
                else
                {
                    Response.WriteFile(filePath);
                }

                Response.Flush();
                Response.Close();
                Response.End();
            }
        }

        [HttpPost]
        public ActionResult ShareLinks(string ids, string DmsEmailReceivers, string DmsEmailMessage)
        {
            var dmsRepo = db.As<DmsRepository>();
            string[] listofIds = ids.Split(',');

            var filesOnly = db.QuerySql<DmsItem>("SELECT Id, Title, Extension FROM DmsRepository WHERE Id IN(@Id) AND ResourceType=0", new { Id = listofIds });

            if (filesOnly.Count > 0)
            {
                string lol = "<ul>";
                foreach (var i in filesOnly)
                {
                    lol += "<li><a href='" + BaseUrl.GetBaseUrl() + "/dms/v/" + i.Id + "'>" + i.Filename + "</a></li>";
                }
                lol += "</ul>";

                Emailer.Send(DmsEmailReceivers, EmailTemplateType.ShareDmsLinks, new { Message = DmsEmailMessage.Replace(Environment.NewLine, "<br />"), ListofLinks = lol });
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult ShareAttachment(string ids, string DmsEmailReceivers, string DmsEmailMessage)
        {
            var dmsPath = Server.MapPath("~/app_data/dms/");
            var listOfIds = ids.Split(',');

            var dmsRepo = db.As<DmsRepository>();

            var files = db.QuerySql<DmsItem>("SELECT Id, Title, Extension, ResourceType, ContentType, ParentId FROM DmsRepository WHERE Id IN (@Id)", new { Id = listOfIds });

            var attachment = new List<System.Net.Mail.Attachment>();

            if (files.Count == 1 && files[0].ResourceType != DmsResourceType.Folder)
            {
                if (System.IO.File.Exists(dmsPath + files[0].Id))
                {
                    attachment.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(System.IO.File.ReadAllBytes(dmsPath + files[0].Id)), files[0].Filename, files[0].ContentType));
                }
            }
            else
            {
                attachment.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(dmsRepo.ZipBytes(ids)), "dms-attachment.zip", "application/zip"));
            }

            Emailer.Send(DmsEmailReceivers, EmailTemplateType.ShareDmsAttachment, new { Message = DmsEmailMessage.Replace(Environment.NewLine, "<br />") }, attachment);
            return Json(true);
        }
    }
}