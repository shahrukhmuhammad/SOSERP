using BaseApp.Entity;
using BaseApp.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.Listing.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All)]
    public class TenderController : AppController
    {
           
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult All()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }
    }
}