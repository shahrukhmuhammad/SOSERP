using BaseApp.Entity;
using BaseApp.System;
using CMS.Logic;
using System.Web.Mvc;
using Insight.Database;
using HRMS;

namespace WebApp.Areas.HRMS.Controllers
{
    [ModuleActivator, AppAuthorize(AppPermission.All)]
    //[AllowAnonymous]
    public class DashboardController : AppController
    {
        private EmployeeEntity employeeRepo;


        public DashboardController()
        {
            employeeRepo = new EmployeeEntity(); //db.As<IEmployee>();
        }
        public ActionResult Index()
        {
            #region CMS
            //ViewBag.EmployeeList = employeeRepo.GetAllEmployees();
            //ViewBag.RecentSlides = webSlide.GetAll();
            //ViewBag.RecentContents = webContent.GetAll();
            //ViewBag.RecentNews = webNews.GetAll();
            //ViewBag.RecentFiles = webFile.GetAll();
            #endregion
            return View();
        }
    }
}