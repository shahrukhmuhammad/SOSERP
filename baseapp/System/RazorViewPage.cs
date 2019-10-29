using System.Security.Claims;
using System.Web.Mvc;

namespace BaseApp.Mvc
{
    public abstract class AppViewPage<TModel> : WebViewPage<TModel>
    {
        protected IAppModule AppModule { get; private set; }
        protected IAppSettings AppSettings { get; private set; }
        protected IUuid Uuid { get; private set; }
        public AppViewPage()
        {
            var dr = DependencyResolver.Current;
            AppSettings = dr.GetService<IAppSettings>();
            Uuid = dr.GetService<IUuid>();
            AppModule = dr.GetService<IAppModule>();
        }
        protected AppPrincipal CurrentUser
        {
            get
            {
                return new AppPrincipal(User as ClaimsPrincipal);
            }
        }

        protected WebPrincipal CurrentWebUser
        {
            get
            {
                return new WebPrincipal(User as ClaimsPrincipal);
            }
        }
    }

    public abstract class AppViewPage : AppViewPage<dynamic>
    {
    }
}
