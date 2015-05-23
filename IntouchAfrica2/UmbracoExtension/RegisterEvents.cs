using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using umbraco.cms.presentation;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.web;
using Umbraco.Core.Persistence;
using IntouchAfrica2.DataAccess;
using IntouchAfrica2.App_Start;
using System.Web.Optimization;

namespace IntouchAfrica2.UmbracoExtension
{
    public class RegisterEvents : ApplicationEventHandler
    {
        private Dictionary<string, Type> _tables = new Dictionary<string, Type>() {
            {"AbsenteeNote", typeof(AbsenteeNote)},
            {"AttendanceRecord", typeof(AttendanceRecord)},
            {"AttendanceSet", typeof(AttendanceSet)},
            {"Contact", typeof(Contact)},
            {"StructuralGroup", typeof(StructuralGroup)},
            {"GroupMember", typeof(GroupMember)},
        };

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var db = applicationContext.DatabaseContext.Database;
            foreach(var pair in _tables.Where(t=>!db.TableExist(t.Key)))
                db.CreateTable(false, pair.Value);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

    }
}