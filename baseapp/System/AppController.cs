using System.Data;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;

namespace BaseApp.System
{
    public abstract class AppController : Controller
    {
        protected IAppModule AppModule;
        protected IAppSettings AppSettings;
        protected IDbConnection db;
        protected IEmailService Emailer;
        protected readonly IUuid Uuid;

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

        public AppController()
        {
            var dr = DependencyResolver.Current;
            AppSettings = dr.GetService<IAppSettings>();
            db = dr.GetService<IDbConnection>();
            Emailer = dr.GetService<IEmailService>();
            Uuid = dr.GetService<IUuid>();
            AppModule = dr.GetService<IAppModule>();
        }

        protected string RenderPartialView(string viewName = null, object model = null)
        {
            if (string.IsNullOrEmpty(viewName)) viewName = RouteData.Values["action"].ToString();
            if (model != null)
            {
                ViewData.Model = model;
            }
            using (var sw = new StringWriter())
            {
                var res = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var context = new ViewContext(ControllerContext, res.View, ViewData, TempData, sw);
                res.View.Render(context, sw);
                return sw.ToString();
            }
        }

        protected string RenderView(string viewName = null, object model = null)
        {
            if (string.IsNullOrEmpty(viewName)) viewName = RouteData.Values["action"].ToString();
            if (model != null)
            {
                this.ViewData.Model = model;
            }
            using (var sw = new StringWriter())
            {
                var res = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
                var context = new ViewContext(ControllerContext, res.View, ViewData, TempData, sw);
                res.View.Render(context, sw);
                return sw.ToString();
            }
        }

        protected FileContentResult ImageResult(string file, int? width = null, int? height = null)
        {
            var img = new WebImage(file);
            if (width.HasValue && height.HasValue)
            {
                img.Resize(width.Value, height.Value);
                img.Crop(1, 1);
            }
            return File(img.GetBytes(), img.ImageFormat);
        }

        protected ContentResult CssViewResult(string viewName = null, object model = null)
        {

            var html = RenderView(viewName, model);
            return Content(html.StripHtml(), "text/css");
        }

        protected ContentResult JsViewResult(string viewName = null, object model = null)
        {
            var html = RenderView(viewName, model);
            return Content(html.StripHtml(), "application/javascript");
        }

        protected FileStreamResult Pdf(string fileDownloadName = null)
        {
            return Pdf(null, null);
        }
        protected FileStreamResult Pdf(string viewName, string fileDownloadName = null)
        {
            return Pdf(viewName, null);
        }

        protected FileStreamResult Pdf(object model, string fileDownloadName = null)
        {
            var viewName = RouteData.Values["action"].ToString();
            return Pdf(viewName, model);
        }

        protected FileStreamResult Pdf(string viewName, object model, string fileDownloadName = null)
        {
            var html = RenderPartialView(viewName, model);
            var bytes = Encoding.UTF8.GetBytes(html);
            var input = new MemoryStream(bytes);
            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 20, 20);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;
            document.Open();
            var xmlWorker = iTextSharp.tool.xml.XMLWorkerHelper.GetInstance();
            xmlWorker.ParseXHtml(writer, document, input, inCssFile: null);
            document.Dispose();
            input.Dispose();
            output.Position = 0;
            return File(output, "application/pdf", fileDownloadName);
        }

        protected byte[] RenderPdf(string viewName = null, object model = null)
        {
            var html = RenderPartialView(viewName, model);
            var bytes = Encoding.UTF8.GetBytes(html);
            var input = new MemoryStream(bytes);
            var output = new MemoryStream();
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 20, 20);
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, output);
            writer.CloseStream = false;
            document.Open();
            var xmlWorker = iTextSharp.tool.xml.XMLWorkerHelper.GetInstance();
            xmlWorker.ParseXHtml(writer, document, input, inCssFile: null);
            document.Dispose();
            input.Dispose();
            output.Position = 0;
            return output.ToArray();
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
            };
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
