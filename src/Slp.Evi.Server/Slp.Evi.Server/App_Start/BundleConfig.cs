using System.Web.Optimization;

namespace Slp.Evi.Server
{
    /// <summary>
    /// Bundle configuration
    /// </summary>
    public static class BundleConfig
    {
        /// <summary>
        /// Registers the bundles.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Content/js/jquery").Include(
                 "~/Content/js/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/Content/js/bootstrap").Include(
                 "~/Content/js/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/Content/js/base").Include(
                 "~/Content/js/base.js"));


            bundles.Add(new StyleBundle("~/Content/css/bootstrap").Include(
                "~/Content/css/bootstrap*").Include(
                "~/Content/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/css/style").Include(
                "~/Content/css/style.css"));
        }
    }
}