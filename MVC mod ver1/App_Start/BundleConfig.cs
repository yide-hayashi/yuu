using System.Web;
using System.Web.Optimization;

namespace WebApplication1
{
    public class BundleConfig
    {
        // バンドルの詳細については、http://go.microsoft.com/fwlink/?LinkId=301862  を参照してください
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.11.0.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 開発と学習には、Modernizr の開発バージョンを使用します。次に、実稼働の準備が
            // できたら、http://modernizr.com にあるビルド ツールを使用して、必要なテストのみを選択します。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/manager").Include(
                      "~/Scripts/manager/jquery.main.js"));
            bundles.Add(new ScriptBundle("~/bundles/edslider").Include(
                     "~/Scripts/jquery.edslider.js",
                      "~/Scripts/fancybox/jquery.mousewheel-3.0.6.pack.js"
                      )
                     );
            bundles.Add(new StyleBundle("~/Content/manager/css").Include(
                      "~/Content/manager/all.css",
                      "~/Content/manager/ie.css",
                      "~/Content/manager/index_manager.css",
                      "~/Content/manager/option.css"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/index.css",
                      "~/Content/layout.css",
                      "~/Content/edslider.css",
                      "~/Content/Prefecture.css"));
        }
    }
}
