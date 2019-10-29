using AspNet.Mvc.Theming;
using AutoMapper;
using HRMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace WebApp
{
    public class SessionThemeResover : IThemeResolver
    {
        public string Resolve(ControllerContext controllerContext, string theme)
        {
            string result;

            if (controllerContext.HttpContext.Session != null && controllerContext.HttpContext.Session["Theme"] != null)
            {
                result = controllerContext.HttpContext.Session["Theme"].ToString();
            }
            else
            {
                result = (!string.IsNullOrEmpty(theme) ? theme : "KlickAndGo");
            }

            return result;
        }
    }

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //Configure Automapper
            AutoMapperConfig.Intialization();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            ThemeManager.Instance.Configure(config => {
                config.ThemeDirectory = "~/Themes";
                config.DefaultTheme = "KlickAndGo";
                config.ThemeResolver = new SessionThemeResover();
            });
        }

        //protected void Application_AuthenticateRequest(object sender, EventArgs e)
        //{
        //    var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
        //    if (authCookie == null) return;
        //    var ticket = FormsAuthentication.Decrypt(authCookie.Value);
        //    var appId = new AppIdentity(ticket);
        //    var principal = new AppPrincipal(appId);
        //    Context.User = principal;
        //    System.Threading.Thread.CurrentPrincipal = principal;
        //}
    }
}
