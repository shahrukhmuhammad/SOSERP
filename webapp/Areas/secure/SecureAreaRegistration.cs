using System.Web.Mvc;

namespace WebApp.Areas.Secure
{
    public class SecureAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Secure";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Secure_default",
                "Secure/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}