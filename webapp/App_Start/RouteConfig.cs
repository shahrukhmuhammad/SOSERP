using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            //routes.MapRoute(
            // name: "Default",
            // url: "Secure/{action}/{id}",
            // defaults: new { Areas = "Secure", controller = "Account", action = "Login", id = UrlParameter.Optional },
            // namespaces: new[] { "WebApp.Controllers" }
            //);

            //routes.MapRoute(
            //    name: "Portal",
            //    url: "Portal/{action}/{id}",
            //    defaults: new { controller = "Portal", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "WebApp.Controllers" }
            //);

            //routes.MapRoute(
            //    name: "All",
            //    url: "{*slug}",
            //    defaults: new { controller = "Cms", action = "Index", slug = UrlParameter.Optional },
            //    namespaces: new[] { "WebApp.Controllers" }
            //);

            //routes.MapRoute(
            //    name: "Ecommerce",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapRoute(
                name: "Default",
                url: "HRMS/{controller}/{action}/{id}",
                defaults: new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
