using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.Secure.Controllers
{
    public class TimeSheetController : Controller
    {
        // GET: Secure/TimeSheet
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(Guid id)
        {
            return View();
        }
    }
}