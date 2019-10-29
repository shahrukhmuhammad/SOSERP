using System.Web.Mvc;

namespace WebApp.Areas.DMS
{
    public class DMSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DMS_default",
                "DMS/{controller}/{action}/{id}",
                new { controller = "Docs", action = "Personal", id = UrlParameter.Optional }
            );
        }
    }
}