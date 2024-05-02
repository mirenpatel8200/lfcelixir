using System.Web.Optimization;

namespace Elixir
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js", 
            //            "~/Scripts/jquery-ui.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          //"~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            // Used in result html.
            bundles.Add(new ScriptBundle("~/bundles/modalpopup").Include(
                     "~/Scripts/jquery-{version}.js",
                     "~/Scripts/bootstrap.js",
                     "~/Scripts/App/modalpopup.js"));

            // Includes summernote and filebrowser plugin.
            bundles.Add(new ScriptBundle("~/bundles/summernote/js").Include(
                "~/Scripts/summernote/summernote-bs4.js",
                "~/Scripts/summernote/plugin/filebrowser/summernote-ext-browser.js"
                ));
            bundles.Add(new StyleBundle("~/bundles/summernote/css").Include(
                //"~/Scripts/summernote/summernote-bs4.css",
                "~/Scripts/summernote/summernote-custom.css"
            ));

            //bundles.Add(new StyleBundle("~/Styles/scss").Include(
            //    "~/Styles/common.css",
            //    "~/Styles/Layout/banner.css",
            //    "~/Styles/Pages/main.css",
            //    "~/Styles/Pages/booksections.css"
            //    ));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //          //"~/Content/bootstrap.css"
            //    ));
        }
    }
}
