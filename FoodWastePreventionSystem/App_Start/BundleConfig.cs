using System.Web;
using System.Web.Optimization;

namespace FoodWastePreventionSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/admin/styles").Include(
                

                "~/Areas/Managers/assets/plugins/morris/morris.css",
                "~/Areas/Managers/assets/css/bootstrap.css",
                "~/Areas/Managers/assets/css/components.css",
                "~/Areas/Managers/assets/css/core.css",
                "~/Areas/Managers/assets/css/icons.css",
                "~/Areas/Managers/assets/css/ionicons.css",
                "~/Areas/Managers/assets/css/pages.css",
                 "~/Areas/Managers/assets/plugins/bs-datepicker/datepicker.css",
                "~/Areas/Managers/assets/css/responsive.css",
                "~/Areas/Managers/assets/plugins/bs-datepicker/datepicker.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/admin/scripts").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.unobtrusive-ajax.min.js",

                 //"~/Areas/Managers/assets/js/jquery.min.js",
                 "~/Scripts/jquery.validate*",
                "~/Areas/Managers/assets/js/bootstrap.min.js",
                "~/Areas/Managers/assets/js/pace.min.js",
                 "~/Areas/Managers/assets/js/loader.js",
                 "~/Areas/Managers/assets/js/detect.js",
                 "~/Areas/Managers/assets/js/fastclick.js",
                  "~/Areas/Managers/assets/js/waves.js",
                 "~/Areas/Managers/assets/js/wow.min.js",
                 "~/Areas/Managers/assets/js/jquery.slimscroll.js",
                 "~/Areas/Managers/assets/js/jquery.nicescroll.js",
                 "~/Areas/Managers/assets/js/jquery.scrollTo.min.js",
                 "~/Areas/Managers/assets/pages/jquery.todo.js",
                 "~/Areas/Managers/assets/plugins/moment/moment.js",
                 "~/Areas/Managers/assets/plugins/morris/morris.min.js",
                 "~/Areas/Managers/assets/plugins/raphael/raphael-min.js",
                 "~/Areas/Managers/assets/plugins/jquery-sparkline/jquery.sparkline.min.js",
                 "~/Areas/Managers/assets/pages/jquery.charts-sparkline.js",
                 "~/Areas/Managers/assets/js/jquery.app.js",
                "~/Areas/Managers/assets/js/cb-chart.js",
                 "~/Areas/Managers/assets/js/jquery.circle-diagram.js",
                 "~/Areas/Managers/assets/plugins/bs-datepicker/datepicker.js",
                "~/Areas/Managers/assets/js/main.js",


                 "~/Areas/Managers/assets/js/fullcalendar.js",
                 "~/Areas/Managers/assets/js/jquery.flot.js",
                 "~/Areas/Managers/assets/js/jquery.flot.pie.js",

                 "~/Areas/Managers/assets/js/modernizr.min.js",
                 "~/Areas/Managers/assets/plugins/bs-datepicker/datepicker.js",
                  "~/Areas/Managers/assets/js/vue.js"



                ));
        }
    }
}
