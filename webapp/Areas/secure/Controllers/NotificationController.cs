using BaseApp.Entity;
using BaseApp.Logic;
using BaseApp.System;
using WebApp.Hubs;
using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Areas.Secure.Controllers
{
    [AppAuthorize(AppPermission.All)]
    public class NotificationController : AppController
    {
        RealTimeHub realtime = new RealTimeHub();

        private IAppUser appUserRepo;
        private IAppLog appLog;
        private IInsurance insuranceRepo;
        private IAppNotification notify;
        private ITaxTypes taxTypeRepo;
        private IBenefitType btRepo;
        private IBonusType bonusTypeRepo;
        private CertificateSettingsRepository certificateSettingsRepo;
        CertificationsRepository certificatesRepo;
        private IAppSMTP appSmtp;
        private IAppRole appRole;
        //IContactRepository contactsRepo;

        public NotificationController()
        {
            appUserRepo = db.As<IAppUser>();
            appLog = db.As<IAppLog>();
            insuranceRepo = db.As<IInsurance>();
            notify = db.As<IAppNotification>();
            taxTypeRepo = db.As<ITaxTypes>();
            btRepo = db.As<IBenefitType>();
            bonusTypeRepo = db.As<IBonusType>();
            certificateSettingsRepo = db.As<CertificateSettingsRepository>();
            certificatesRepo = db.As<CertificationsRepository>();
            appSmtp = db.As<IAppSMTP>();
            appRole = db.As<IAppRole>();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Configurations()
        {
            ViewBag.NotificationsList = notify.GetAllByModule("Ecommerce");
            return View();
        }

        public ActionResult _NotificationsRecord(Guid Id)
        {
            var model = new NotificaitionSettings();
            model = notify.GetNotifyById(Id);

            ViewBag.ApplyForList = new SelectList(Enum.GetValues(typeof(ApplyFor))
                                                      .Cast<ApplyFor>().Select(e => new
                                                      {
                                                          value = e,
                                                          text = e.ToString().ToSpacedTitleCase()
                                                      }).ToList(), "value", "text");
            ViewBag.AllRoles = appRole.GetAll().Where(x => x.IsSystem == false).ToList();
            //ViewBag.AppUserRole = new SelectList(Enum.GetValues(typeof(AppUserRole))
            //                                          .Cast<AppUserRole>().Select(e => new
            //                                          {
            //                                              value = e,
            //                                              text = e.ToString().ToSpacedTitleCase()
            //                                          }).ToList(), "value", "text");
            return PartialView(model);
        }

        public ActionResult Settings()
        {
            ViewBag.Model = appSmtp.GetAll();
            ViewBag.Modules = appSmtp.GetModules();
            return View();
        }
    }
}