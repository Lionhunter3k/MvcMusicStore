using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using NHibernate;
using NHibernate.Stat;
using MvcMusicStore.Models.Tests.SessionFactoryBuilders;

namespace MvcMusicStore.Models.Tests
{
    public abstract class Fixture
    {
        protected static ILog Logger { get; private set; }
        protected static ISessionFactory SessionFactory { get; private set; }
        protected IStatistics Statistics { get { return SessionFactory.Statistics; } }

        static Fixture()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger = LogManager.GetLogger(typeof(Fixture));

            SetSessionFactory();
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.InitializeOfflineProfiling("c:\\temp\\nhprof.nhprof");
        }

        private static void SetSessionFactory()
        {

            SessionFactory = new SQLiteHbmSessionFactoryBuilder().BuildSessionFactory();
        }

        protected static ISession CreateSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
