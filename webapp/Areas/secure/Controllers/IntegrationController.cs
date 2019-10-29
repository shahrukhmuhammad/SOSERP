using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Hubs;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize(AppPermission.All)]
    public class IntegrationController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();
        private IAppUser appUserRepo;
        private IAppLog appLog;        
        private IAppNotification notify;

        public IntegrationController()
        {
            appUserRepo = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            notify = db.As<IAppNotification>();

        }
        // GET: Secure/Integration
        public ActionResult PaymentGateway()
        {
            return View();
        }
    }
}