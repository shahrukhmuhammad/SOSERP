using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.Secure.Controllers
{
    public class TestDummyController : Controller
    {
        // GET: Secure/TestDummy
        public ActionResult Index()
        {
            return View();
        }
    }
}