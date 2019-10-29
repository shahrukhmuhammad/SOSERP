using System.Web;
using System.Web.Optimization;

namespace WebApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Styles

            #region Base App
            bundles.Add(new StyleBundle("~/Content/BaseApp").Include(
                      "~/Content/Font Awesome/font-awesome.css",
                      "~/Content/Bootstrap Sidebar/sidebar.css",
                      "~/Content/Bootstrap Notify/bootstrap-notify.css",
                      "~/Content/Bootstrap DateTimePicker/bootstrap-datetimepicker.css",
                      "~/Content/Bootstrap Select/bootstrap-select.css",
                      "~/Content/jQuery DataTable/jquery.dataTables.css",
                      "~/Content/jQuery DataTable/dataTables.bootstrap.css",
                      "~/Content/Bootstrap Switch/bootstrap-switch.css"));

            bundles.Add(new StyleBundle("~/Content/CustomBaseApp").Include(
                      "~/Content/BaseApp.css"));

            bundles.Add(new StyleBundle("~/Content/FontAwesome").Include("~/Content/Font Awesome/font-awesome.css"));
            #endregion



            bundles.Add(new StyleBundle("~/Content/SummerNote").Include(
                      "~/Content/Summer Note/summernote.css"));


            bundles.Add(new StyleBundle("~/Content/AlloyEditor").Include("~/Content/alloy-editor/assets/alloy-editor-ocean.css"));

            #region Cms
            bundles.Add(new StyleBundle("~/Content/Cms").Include(
                      "~/Content/Bootstrap/bootstrap-SpacelabTheme.css",
                      "~/Content/Font Awesome/font-awesome.css",
                      "~/Content/Cms.css"));
            #endregion


            #endregion

            #region Scripts
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));


            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jQuery Validate/jquery.validate*"));

            #region Base App
            bundles.Add(new ScriptBundle("~/bundles/BaseApp").Include(
                        "~/Scripts/Bootstrap Sidebar/sidebar.js",
                        "~/Scripts/Bootstrap Switch/bootstrap-switch.js",
                        "~/Scripts/Bootstrap Notify/bootstrap-notify.js",
                        "~/Scripts/Bootstrap DateTimePicker/moment.js",
                        "~/Scripts/Bootstrap DateTimePicker/bootstrap-datetimepicker.js",
                        "~/Scripts/Bootstrap Select/bootstrap-select.js",
                        "~/Scripts/Bootbox/bootbox.js",
                        "~/Scripts/jQuery DataTable/jquery.dataTables.js",
                        "~/Scripts/jQuery DataTable/dataTable.bootstrap.js",
                        "~/Scripts/jQuery DataTable/jquery.datatables.customsearch.js",
                        "~/Scripts/Input Mask/jquery.inputmask.bundle.js"));

            bundles.Add(new ScriptBundle("~/bundles/CustomBaseApp").Include("~/Scripts/BaseApp.js"));
            #endregion

            #region CMS
            bundles.Add(new ScriptBundle("~/bundles/CustomCms").Include(
                "~/Scripts/Input Mask/jquery.inputmask.bundle.js",
                "~/Scripts/Cms.js"));
            #endregion
            //bundles.Add(new ScriptBundle("~/bundles/CustomEcommerce").Include(
            //    "~/Scripts/App/EcommerceSettings.js"));
            #region Ecommerce

            #endregion
            bundles.Add(new ScriptBundle("~/bundles/SummerNote").Include("~/Scripts/Summer Note/summernote.js"));

            bundles.Add(new ScriptBundle("~/bundles/AlloyEditor").Include("~/Content/alloy-editor/alloy-editor-all.js"));



            bundles.Add(new ScriptBundle("~/bundles/Bootstrap").Include(
                      "~/Scripts/Bootstrap/bootstrap.js",
                      "~/Scripts/Respond/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/BootstrapNewsTicker").Include("~/Scripts/Bootstrap News Ticker/jquery.bootstrap.newsbox.js"));
            #endregion
        }
    }
}
