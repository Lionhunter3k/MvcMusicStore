using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Cfg;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace MvcMusicStore.Infrastructure.Persistence
{
    public static class NHConfigurator
    {
        public static string CONN_STR =
          "Data Source=:memory:;Version=3;New=True;";

        private static Configuration _configuration;
        private static ISessionFactory _sessionFactory;

        public static void CreateTestDbUsing(string connString)
        {
            _configuration = new Configuration().Configure()
           .DataBaseIntegration(db =>
           {
               db.Dialect<SQLiteDialect>();
               db.Driver<SQLite20Driver>();
               db.ConnectionProvider<TestConnectionProvider>();
               db.ConnectionString = connString;
               db.BatchSize = 20;
           })
           .SetProperty(NHibernate.Cfg.Environment.CurrentSessionContextClass,
             "thread_static");

            var props = _configuration.Properties;
            if (props.ContainsKey(NHibernate.Cfg.Environment.ConnectionStringName))
                props.Remove(NHibernate.Cfg.Environment.ConnectionStringName);

            _sessionFactory = _configuration.BuildSessionFactory();
        }

        static NHConfigurator()
        {
            _configuration = new Configuration().Configure()
              .DataBaseIntegration(db =>
              {
                  db.Dialect<SQLiteDialect>();
                  db.Driver<SQLite20Driver>();
                  db.ConnectionProvider<TestConnectionProvider>();
                  db.ConnectionString = CONN_STR;
                  db.BatchSize = 20;
              })
              .SetProperty(NHibernate.Cfg.Environment.CurrentSessionContextClass,
                "thread_static");

            var props = _configuration.Properties;
            if (props.ContainsKey(NHibernate.Cfg.Environment.ConnectionStringName))
                props.Remove(NHibernate.Cfg.Environment.ConnectionStringName);

            _sessionFactory = _configuration.BuildSessionFactory();
        }

        public static Configuration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        public static ISessionFactory SessionFactory
        {
            get
            {
                return _sessionFactory;
            }
        }

    }
}
