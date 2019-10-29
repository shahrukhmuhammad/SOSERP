using System.Web.Mvc;

namespace WebApp.Areas.CMS
{
    public class CMSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CMS_default",
                "CMS/{controller}/{action}/{id}",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}