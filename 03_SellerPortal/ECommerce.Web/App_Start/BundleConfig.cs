using System.Web;
using System.Web.Optimization;

namespace ECommerce.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();
            // Metronic Style
            bundles.Add(new StyleBundle("~/Content/themes/metronic/css").Include(
                // Global Mandatory Style
                "~/Content/themes/metronic/assets/global/plugins/font-awesome/css/font-awesome.min.css",
                "~/Content/themes/metronic/assets/global/plugins/simple-line-icons/simple-line-icons.min.css",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap/css/bootstrap.min.css",
                "~/Content/themes/metronic/assets/global/plugins/uniform/css/uniform.default.css",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css",
                "~/Content/themes/metronic/assets/global/plugins/jstree/dist/themes/default/style.min.css",
                // Metronic Theme Style
                "~/Content/themes/metronic/assets/global/css/components.css",
                "~/Content/themes/metronic/assets/global/css/plugins.css",
                "~/Content/themes/metronic/assets/admin/layout/css/layout.css",
                "~/Content/themes/metronic/assets/admin/layout/css/themes/default.css",
                "~/Content/themes/metronic/assets/admin/layout/css/custom.css"
                ));

            // Metronic Script
            bundles.Add(new ScriptBundle("~/Content/themes/metronic/js").Include(
                "~/Content/themes/metronic/assets/global/plugins/jquery-1.11.0.min.js",
                 "~/Content/themes/metronic/assets/global/plugins/bootbox/bootbox.min.js",

                //"~/Scripts/jquery-{version}.min.js",
                //"~/Scripts/jquery-ui.min-{version}.js",
                "~/Content/themes/metronic/assets/global/plugins/jquery-migrate-1.2.1.min.js",
                "~/Content/themes/metronic/assets/global/plugins/jquery-validation/js/jquery.validate.min.js",
                "~/Content/themes/metronic/assets/global/plugins/jquery-ui/jquery-ui-1.10.3.custom.min.js",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap/js/bootstrap.min.js",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js",
                "~/Content/themes/metronic/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/Content/themes/metronic/assets/global/plugins/jquery.blockui.min.js",
                "~/Content/themes/metronic/assets/global/plugins/jquery.cokie.min.js",
                "~/Content/themes/metronic/assets/global/plugins/uniform/jquery.uniform.min.js",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                "~/Content/themes/metronic/assets/global/plugins/jquery-serializeJSON/jquery.serializejson.min.js",
                "~/Content/themes/metronic/assets/global/scripts/metronic.js",
                "~/Content/themes/metronic/assets/admin/layout/scripts/layout.js",
                "~/Content/themes/metronic/assets/admin/layout/scripts/quick-sidebar.js",
                "~/Content/themes/metronic/assets/admin/layout/scripts/demo.js",
                "~/Content/scripts/common/jquery-extension.js",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-daterangepicker/moment.min.js",
                "~/Content/themes/metronic/assets/global/plugins/jstree/dist/jstree.min.js"
                ));


            bundles.Add(new StyleBundle("~/Content/plugin/bootstrapValidator/css").Include(
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-validator/css/bootstrapValidator.min.css"));

            bundles.Add(new ScriptBundle("~/Content/plugin/bootstrapValidator/js").Include(
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-validator/js/bootstrapValidator.min.js"));


            bundles.Add(new StyleBundle("~/Content/plugin/datetimepicker/css").Include(
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-datepicker/css/datepicker3.css",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-timepicker/css/bootstrap-timepicker.min.css",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-daterangepicker/daterangepicker-bs3.css",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-datetimepicker/css/datetimepicker.css"
                ));

            bundles.Add(new ScriptBundle("~/Content/plugin/datetimepicker/js").Include(
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-timepicker/js/bootstrap-timepicker.min.js",

                "~/Content/themes/metronic/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.js",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js",
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-datepicker/js/locales/bootstrap-datepicker.zh-CN.js"

                ));

            bundles.Add(new StyleBundle("~/Content/plugin/select/css").Include(
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-select/bootstrap-select.min.css",
                "~/Content/themes/metronic/assets/global/plugins/select2/select2.css",
                "~/Content/themes/metronic/assets/global/plugins/jquery-multi-select/css/multi-select.css"
            ));

            bundles.Add(new ScriptBundle("~/Content/plugin/select/js").Include(
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-select/bootstrap-select.min.js",
                "~/Content/themes/metronic/assets/global/plugins/select2/select2.min.js",
                "~/Content/themes/metronic/assets/global/plugins/jquery-multi-select/js/jquery.multi-select.js"
                ));


            bundles.Add(new StyleBundle("~/Content/plugin/select/css").Include(
                "~/Content/themes/metronic/assets/global/plugins/bootstrap-select/bootstrap-select.min.css",
                "~/Content/themes/metronic/assets/global/plugins/select2/select2.css",
                "~/Content/themes/metronic/assets/global/plugins/jquery-multi-select/css/multi-select.css"
            ));

            bundles.Add(new ScriptBundle("~/Content/plugin/datatables/js").Include(
                "~/Content/themes/metronic/assets/global/plugins/jquery.json.min.js",
                "~/Content/themes/metronic/assets/global/plugins/datatables/media/js/jquery.dataTables.min.js",
                "~/Content/themes/metronic/assets/global/plugins/datatables/plugins/bootstrap/dataTables.bootstrap.js",
                "~/Content/themes/metronic/assets/global/plugins/datatables/extensions/ColVis/js/dataTables.colVis.min.js",
                "~/Content/themes/metronic/assets/global/scripts/datatable.js"
                ));

            bundles.Add(new StyleBundle("~/Content/plugin/datatables/css").Include(
                "~/Content/themes/metronic/assets/global/plugins/datatables/plugins/bootstrap/dataTables.bootstrap.css",
                "~/Content/themes/metronic/assets/global/plugins/datatables/extensions/ColVis/css/dataTables.colVis.min.css"
                ));

            bundles.Add(new ScriptBundle("~/Content/plugin/jstree/js").Include(
               "~/Content/themes/metronic/assets/global/plugins/jstree/dist/jstree.min.js"
               ));
            bundles.Add(new StyleBundle("~/Content/plugin/jstree/css").Include(
               "~/Content/themes/metronic/assets/global/plugins/jstree/dist/themes/default/style.min.css"
               ));
            bundles.Add(new ScriptBundle("~/Content/plugin/form/js").Include(
               "~/Content/third/jquery.form.js"
               ));
        }
    }
}