using BaseApp.Entity;
using BaseApp.System;
using CMS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using CMS.Entity;

namespace WebApp.Areas.CMS.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All, AppPermission.ViewCMS, AppPermission.CMS)]
    public class SubsiteController : AppController
    {
        private ICmsSubsite subsite;
        private ICmsPage webPage;
        
        private ICmsContent webContent;
        private ICmsSlide webSlide;
        private ICmsNews webNews;
        private ICmsFile webFile;

        public SubsiteController()
        {
            subsite = db.As<ICmsSubsite>();
            webPage = db.As<ICmsPage>();
            webContent = db.As<ICmsContent>();
            webSlide = db.As<ICmsSlide>();
            webNews = db.As<ICmsNews>();
            webFile = db.As<ICmsFile>();

            ViewBag.AllPages = webPage.GetAll();
        }

        public ActionResult Index()
        {
            return View();
        }

        #region Record
        public ActionResult Record(Guid? Id, CmsSubsite model)
        {
            if (Id.HasValue)
            {
                model = subsite.GetById(Id.Value);
            }
            else
            {
                model.LandingPage = new CmsPage();
            }
            if (Request.IsPost())
            {
                var id = subsite.Create(model);
            }

            return View(model);
        }

        public ActionResult Dashboard(string id)
        {
            #region CMS
            ViewBag.RecentPages = webPage.GetAll();
            ViewBag.RecentSlides = webSlide.GetAll();
            ViewBag.RecentContents = webContent.GetAll();
            ViewBag.RecentNews = webNews.GetAll();
            ViewBag.RecentFiles = webFile.GetAll();
            #endregion
            return View();
        }
        #endregion
    }
}