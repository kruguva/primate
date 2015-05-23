using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;

namespace IntouchAfrica2.Utilities
{
    public static class DbUtils
    {
        public static DataTable ExecuteDataTable(UmbracoDatabase db, string query)
        {
            db.OpenSharedConnection();
            var command = db.Connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = query;
            return ExecuteDataTable(db, command);
        }

        public static DataTable ExecuteDataTable(UmbracoDatabase db, IDbCommand command)
        {
            var reader = command.ExecuteReader();
            var table = new DataTable();
            table.Load(reader);
            db.CloseSharedConnection();
            return table;
        }

    }
}