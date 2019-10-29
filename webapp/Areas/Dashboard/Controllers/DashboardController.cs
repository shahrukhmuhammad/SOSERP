using BaseApp.System;
using CMS.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Hubs;
using Insight.Database;

namespace WebApp.Areas.Dashboard.Controllers
{
    [AppAuthorize]
    public class DashboardController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private ICmsPage webPage;
        private ICmsContent webContent;
        private ICmsSlide webSlide;
        private ICmsNews webNews;
        private ICmsFile webFile;

        public DashboardController()
        {
            webPage = db.As<ICmsPage>();
            webContent = db.As<ICmsContent>();
            webSlide = db.As<ICmsSlide>();
            webNews = db.As<ICmsNews>();
            webFile = db.As<ICmsFile>();
        }

        public ActionResult Index()
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
    }
}