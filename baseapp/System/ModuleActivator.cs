using System.Web.Mvc;

namespace BaseApp.System
{
    public class ModuleActivatorAttribute : ActionFilterAttribute
    {
        private IAppModule AppModule;
        public ModuleActivatorAttribute()
        {
            AppModule = DependencyResolver.Current.GetService<IAppModule>();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var module = filterContext.RouteData.DataTokens["area"].ToString();
            if (!string.IsNullOrEmpty(module))
            {
                var checkModule = AppModule.GetById(module);
                if (checkModule != null)
                {
                    if (!checkModule.Status)
                    {
                        var message = checkModule.Message;
                        var result = new ContentResult { Content = message, ContentType = "text/html" };
                        filterContext.Result = result;
                    }
                }
            }
        }
    }
}
