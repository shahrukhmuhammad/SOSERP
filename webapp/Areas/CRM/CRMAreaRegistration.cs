﻿using System.Web.Mvc;

namespace WebApp.Areas.CRM
{
    public class CRMAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CRM";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CRM_default",
                "CRM/{controller}/{action}/{id}",
                new { controller = "Contact", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}