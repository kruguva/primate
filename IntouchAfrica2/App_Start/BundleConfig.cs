using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace IntouchAfrica2.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var mainStyleBundle = new StyleBundle("~/bundles/maincss").Include("~/css/main.css");
            var appStyleBundle = new StyleBundle("~/bundles/appcss").Include("~/css/abn_tree.css", "~/css/toaster.css");

            bundles.Add(mainStyleBundle);
            bundles.Add(appStyleBundle);

            var appScriptBundle = new ScriptBundle("~/bundles/appscripts")
                .Include(  //Third Party
                    "~/scripts/vendor/abn_tree_directive.js",
                    "~/scripts/vendor/toaster.js")
                .Include(  //Local
                    "~/App/app.js",
                    "~/App/main.js",
                    "~/App/appController.js",
                    "~/App/apiService.js",

                    //Common
                    "~/App/common/confirmActionController.js",

                    //Structure
                    "~/App/structure/structureService.js",
                    "~/App/structure/structureController.js",

                    //Attendance
                    "~/App/attendance/attendanceService.js",
                    "~/App/attendance/captureAttendanceController.js",
                    "~/App/attendance/teacherAttendanceController.js",
                    "~/App/attendance/studentParentAttendanceController.js",
                    "~/App/attendance/studentAttendanceDirective.js",
                    
                    //Member
                    "~/App/member/memberService.js",
                    "~/App/member/createController.js",
                    "~/App/member/createModalController.js",
                    "~/App/member/editorDirective.js",
                    "~/App/member/memberListController.js",
                    "~/App/member/viewController.js",
                    "~/App/member/memberSelectionController.js"
                    
                    );
            bundles.Add(appScriptBundle);

            bundles.UseCdn = true;

            bundles.Add(new StyleBundle("~/bundles/bootstrap", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.0/css/bootstrap.min.css").Include("~/css/bootstrap.min.css"));
            bundles.Add(new StyleBundle("~/bundles/bootstrap-theme", "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.0/css/bootstrap-theme.min.css").Include("~/css/bootstrap-theme.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/angular", "https://ajax.googleapis.com/ajax/libs/angularjs/1.3.15/angular.min.js").Include("~/scripts/vendor/angular.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-route", "https://ajax.googleapis.com/ajax/libs/angularjs/1.3.15/angular-route.min.js").Include("~/scripts/vendor/angular-route.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-resource", "https://ajax.googleapis.com/ajax/libs/angularjs/1.3.15/angular-resource.min.js").Include("~/scripts/vendor/angular-resource.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-animate", "https://ajax.googleapis.com/ajax/libs/angularjs/1.3.15/angular-animate.min.js").Include("~/scripts/vendor/angular-animate.js"));
            bundles.Add(new ScriptBundle("~/bundles/ui-bootstrap", "https://cdnjs.cloudflare.com/ajax/libs/angular-ui-bootstrap/0.13.0/ui-bootstrap-tpls.min.js").Include("~/scripts/vendor/angular-ui/ui-bootstrap-tpls-0.13.0.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/underscore", "https://cdnjs.cloudflare.com/ajax/libs/underscore.js/1.7.0/underscore-min.js").Include("~/scripts/vendor/underscore-min.js"));
        }
    }
}
