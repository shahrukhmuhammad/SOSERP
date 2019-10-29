﻿using System.Web.Mvc;

namespace WebApp.Areas.Dashboard
{
    public class DashboardAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Dashboard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Dashboard_default",
                //"Dashboard/{controller}/{action}/{id}",
                "Dashboard/{action}/{id}",
                new { action = "Index", controller = "Dashboard", id = UrlParameter.Optional }
            );
        }
    }
}