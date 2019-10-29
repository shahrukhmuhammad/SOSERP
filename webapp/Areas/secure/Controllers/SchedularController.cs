using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.Secure.Controllers
{
    public class SchedularController : Controller
    {
        // GET: Secure/Schedular
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Reader()
        {
            return PartialView();
        }

        public ActionResult Calendar()
        {
            return View();
        }

        public ActionResult _CreateEventBox()
        {
            return PartialView();
        }
    }
}