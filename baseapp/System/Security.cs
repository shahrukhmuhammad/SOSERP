using BaseApp.Entity;
using CRM.Entity;
using System.Linq;
using System.Security.Claims;
using System.Web.Routing;

namespace System.Web.Mvc
{
    public class ImageOutputCacheAttribute : OutputCacheAttribute
    {
        public string ContentType { get; set; }
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            if (string.IsNullOrEmpty(ContentType)) ContentType = "image/jpeg";
            filterContext.HttpContext.Response.ContentType = ContentType;
        }
    }

    public class WebAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized) return false;
            var user = new WebPrincipal(httpContext.User as ClaimsPrincipal);
            if (user.ContactType == ContactType.Customer) return false;
            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Controller.ViewData.Model = new HandleErrorInfo(new System.Security.Authentication.AuthenticationException("Forbidden"),
                    filterContext.RouteData.Values["controller"].ToString(),
                    filterContext.RouteData.Values["action"].ToString());
                filterContext.Result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = filterContext.Controller.ViewData,
                    TempData = filterContext.Controller.TempData
                };

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new ContentResult() { Content = "Unauthorized" };
                    filterContext.HttpContext.Response.StatusCode = 403;
                }
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }

    public class AppAuthorizeAttribute : AuthorizeAttribute
    {
        private string[] permissions;
        public AppAuthorizeAttribute(params string[] permissions)
        {
            this.permissions = permissions;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized) return false;
            var user = new AppPrincipal(httpContext.User as ClaimsPrincipal);
            if (user.Status == AppUserStatus.Suspended) return false;
            if (permissions.Length == 0) return authorized;
            return user.HasAnyPermission(permissions);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Controller.ViewData.Model = new HandleErrorInfo(new System.Security.Authentication.AuthenticationException("Forbidden"),
                    filterContext.RouteData.Values["controller"].ToString(),
                    filterContext.RouteData.Values["action"].ToString());
                filterContext.Result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = filterContext.Controller.ViewData,
                    TempData = filterContext.Controller.TempData
                };

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new ContentResult() { Content = "Unauthorized" };
                    filterContext.HttpContext.Response.StatusCode = 403;
                }
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }

    public class TraderAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized) return false;
            var user = new WebPrincipal(httpContext.User as ClaimsPrincipal);
            if (user.ContactType == ContactType.Customer) return false;
            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string controller = filterContext.RouteData.Values["controller"].ToString();
            filterContext.Result = new RedirectToRouteResult(new
            RouteValueDictionary(new { area = "tradeportal", controller = "account", action = "login" }));
        }
    }
}

namespace System.Security.Claims
{
    using BaseApp.Entity;
    using BaseApp.Logic;
    using CRM.Entity;
    using CRM.Logic;
    using Insight.Database;
    using Web.Mvc;

    public class AppPrincipal : ClaimsPrincipal
    {
        AppUser appUser;

        public AppPrincipal(ClaimsPrincipal principal) : base(principal)
        {
            Data.IDbConnection Db = DependencyResolver.Current.GetService<Data.IDbConnection>();
            var appRole = Db.As<IAppRole>();
            var repo = Db.As<IAppUser>();
            var contactRepo = Db.As<IContact>();
            appUser = repo.GetUserById(Id);

            if (appUser != null)
            {
                appUser.Role = appRole.GetById(appUser.RoleId);
            }
        }

        public string Code
        {
            get
            {
                return appUser.Code;
            }
        }
        public string FullName
        {
            get
            {
                return string.Format("{0} {1} {2}", appUser.FirstName, appUser.MiddleName, appUser.LastName);
            }
        }
        public string Email
        {
            get
            {
                return appUser.Email;
            }
        }
        public string Username
        {
            get
            {
                return appUser.Username;
            }
        }

        public string Theme
        {
            get
            {
                return appUser != null ? appUser.Theme : "";
            }
        }

        public Guid Id
        {
            get
            {
                return Guid.Parse(this.FindFirst(ClaimTypes.Sid).Value);
            }
        }

        public Guid? OfficeId
        {
            get
            {
                return appUser.OfficeId;
            }
        }

        public string Role
        {
            get
            {
                return string.Format("{0} {1}", appUser.Role.Code, appUser.Role.Title);
            }
        }

        public string[] Permissions
        {
            get
            {
                return appUser.Role.PermissionsList;
            }
        }

        public AppUserStatus Status
        {
            get
            {
                return appUser.Status;
            }
        }

        public DateTime LastLoginDate
        {
            get { return appUser.LastLogin; }
        }

        public bool HasAnyPermission(params string[] permissions)
        {
            return appUser.Role.Permissions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Any(x => Array.IndexOf(permissions, x) >= 0);
        }

        public string PageLength
        {
            get
            {
                return appUser.PageLength;
            }
        }
    }

    public class WebPrincipal : ClaimsPrincipal
    {
        Contact webUser;

        public WebPrincipal(ClaimsPrincipal principal) : base(principal)
        {
            Data.IDbConnection Db = DependencyResolver.Current.GetService<Data.IDbConnection>();
            var repo = Db.As<IContact>();
            webUser = repo.GetById(Id);
        }

        public string Code
        {
            get
            {
                return webUser.Code;
            }
        }
        public DateTime MemberSince
        {
            get
            {
                return webUser.CreatedOn;
            }
        }

        //public DateTime LastLoggedIn
        //{
        //    get
        //    {
        //        return webUser.LoginDateTime;
        //    }
        //}
        public string FullName
        {
            get
            {
                if (webUser == null)
                {
                    return "";
                }
                else
                {
                    return string.Format("{0} {1} {2}", webUser.FirstName, webUser.MiddleName, webUser.LastName);
                }
                
            }
        }
        public string Email
        {
            get
            {
                return webUser.Email;
            }
        }

        public Guid Id
        {
            get
            {
                return Guid.Parse(this.FindFirst(ClaimTypes.Sid).Value);
            }
        }

        public ContactType ContactType
        {
            get
            {
                return webUser.ContactType;
            }
        }
    }
}
