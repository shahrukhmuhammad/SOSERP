using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace BaseApp.System
{
    public static class Helper
    {
        public static MvcHtmlString BreadCrumb(this HtmlHelper helper, RequestContext context)
        {
            var area = context.RouteData.DataTokens["area"].ToString();
            var controller = context.RouteData.Values["controller"].ToString();
            var action = context.RouteData.Values["action"].ToString();
            var id = context.RouteData.Values["id"] != null ? " &raquo; " + context.RouteData.Values["id"].ToString() : "";
            var backlink = "<li><a href='javascript://' onclick='window.history.back()'><i class='fa fa-angle-left'></i> Go Back</a></li>";
            var homelink = "<li><a href='/Dashboard'>Dashboard</a></li>";
            var homeicon = "<li><a href='/Dashboard'><i class='fa fa-dashboard'></i></a></li>";
            if (area.Equals("Dashboard", StringComparison.CurrentCultureIgnoreCase) && controller.Equals("Dashboard", StringComparison.CurrentCultureIgnoreCase) && action.Equals("Index", StringComparison.CurrentCultureIgnoreCase))
            {
                return MvcHtmlString.Create(homeicon);
            }
            else if (controller.Equals("Dashboard", StringComparison.CurrentCultureIgnoreCase) && !action.Equals("Index", StringComparison.CurrentCultureIgnoreCase))
            {
                return MvcHtmlString.Create(string.Format("{0} {1} {2}{3}", backlink, homelink, action.ToSpacedTitleCase(), id));
            }
            else if (!controller.Equals("Dashboard", StringComparison.CurrentCultureIgnoreCase) && action.Equals("Index", StringComparison.CurrentCultureIgnoreCase))
            {
                return MvcHtmlString.Create(string.Format("{0}{1} <li class='active'>{2}</li>", backlink, homelink, controller.ToSpacedTitleCase()));
            }
            else
            {
                if (controller.Equals("Dashboard", StringComparison.CurrentCultureIgnoreCase) && action.Equals("Index", StringComparison.CurrentCultureIgnoreCase))
                {
                    return MvcHtmlString.Create(string.Format("{0}{1} <li><a href='/" + area + "/" + controller + "'>" + area.ToSpacedTitleCase() + " {2}</a></li> <li class='active'>{3}</li>", backlink, homelink, controller.ToSpacedTitleCase(), action.ToSpacedTitleCase(), id));
                }
                else
                {
                    return MvcHtmlString.Create(string.Format("{0}{1} <li><a href='/" + area + "/" + controller + "'>{2}</a></li> <li class='active'>{3}</li>", backlink, homelink, controller.ToSpacedTitleCase(), action.ToSpacedTitleCase(), id));
                }
            }
        }
    }
}
