﻿using System.Web.Mvc;

namespace WebApp.Areas.Operations
{
    public class OperationsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Operations";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Operations_default",
                "Operations/{controller}/{action}/{id}",
                new { controller = "Account", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}