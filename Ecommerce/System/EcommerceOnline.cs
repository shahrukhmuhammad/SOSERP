using BaseApp;
using System.Web.Mvc;

namespace Ecommerce.System
{
    public class WebsiteOnlineAttribute : ActionFilterAttribute
    {
        private IAppSettings AppSettings;

        public WebsiteOnlineAttribute()
        {
            AppSettings = DependencyResolver.Current.GetService<IAppSettings>();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var offline = !AppSettings.GetVal<bool>("website:Online");
            if (offline)
            {
                var message = AppSettings.GetVal("website:OfflineMessage");
                var result = new ContentResult { Content = message, ContentType = "text/html" };
                filterContext.Result = result;
            }
        }
    }
}
