using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insight.Database;
using BaseApp.Entity;
using BaseApp.System;
using BaseApp.Logic;
using WebApp.Hubs;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize(AppPermission.All, AppPermission.ViewContact, AppPermission.Contact)]
    public class DocumentationController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppUser appUser;
        private IAppLog appLog;
        private IOffice ofcRepo;
        private IAppNotification notify;
        private IAppRole appRole;
        public DocumentationController()
        {
            appUser = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            ofcRepo = db.As<IOffice>();
            notify = db.As<IAppNotification>();
            appRole = db.As<IAppRole>();

            ViewBag.AllOffices = ofcRepo.GetAll();
        }

        
        public ActionResult Index()
        {
            return View();
        }
    }
}