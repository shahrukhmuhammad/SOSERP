using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.CMS.Controllers
{
    public class DesignController : Controller
    {
        // GET: CMS/Design
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Themes()
        {
            return View();
        }
    }
}