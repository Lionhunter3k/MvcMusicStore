using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using NHibernate.Connection;

namespace MvcMusicStore.Infrastructure.Persistence
{
    public class TestConnectionProvider : DriverConnectionProvider
    {
        [ThreadStatic]
        private static IDbConnection Connection;

        public static void CloseDatabase()
        {
            if (Connection != null)
                Connection.Dispose();
            Connection = null;
        }

        public override IDbConnection GetConnection()
        {
            if (Connection == null)
            {
                Connection = Driver.CreateConnection();
                Connection.ConnectionString = ConnectionString;
                Connection.Open();
            }
            return Connection;
        }

        public override void CloseConnection(IDbConnection conn)
        {
            // Do nothing
        }
    }
}
