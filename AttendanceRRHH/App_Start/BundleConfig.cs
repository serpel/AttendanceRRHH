using System.Web;
using System.Web.Optimization;

namespace AttendanceRRHH
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            bundles.Add(new ScriptBundle("~/bundles/javascripts").Include(
                "~/Scripts/jquery-2.2.2.min.js",
                "~/Scripts/jquery-ui-1.11.4.min.js",
                "~/Content/assets/plugins/boostrapv3/js/bootstrap.min.js",
                "~/Content/assets/plugins/breakpoints.js",
                "~/Content/assets/plugins/jquery-unveil/jquery.unveil.min.js",
                "~/Content/assets/plugins/jquery-block-ui/jqueryblockui.js",
                "~/Content/assets/plugins/jquery-scrollbar/jquery.scrollbar.min.js",
                //"~/Content/assets/plugins/pace/pace.min.js",
                "~/Content/assets/plugins/jquery-numberAnimate/jquery.animateNumbers.js",
                "~/Content/assets/js/core.js",
                //"~/Content/assets/js/chat.js",
                "~/Content/assets/js/demo.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                // "~/Content/assets/plugins/pace/pace-theme-flash.css",
                "~/Content/assets/plugins/jquery-scrollbar/jquery.scrollbar.css",
                "~/Content/assets/plugins/boostrapv3/css/bootstrap.min.css",
                "~/Content/assets/plugins/boostrapv3/css/bootstrap-theme.min.css",
                "~/Content/assets/plugins/font-awesome/css/font-awesome.css",
                "~/Content/assets/css/animate.min.css",
                "~/Content/assets/css/style.css",
                "~/Content/assets/css/responsive.css",
                "~/Content/assets/css/custom-icon-set.css"));
        }
    }
}
