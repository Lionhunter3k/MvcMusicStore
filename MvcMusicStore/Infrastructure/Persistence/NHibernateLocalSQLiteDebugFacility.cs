using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel.Facilities;
using System.Data.SQLite;
using NHibernate.Tool.hbm2ddl;
using Castle.MicroKernel.Registration;
using NHibernate;
using System.IO;

namespace MvcMusicStore.Infrastructure.Persistence
{
    public class NHibernateLocalSQLiteDebugFacility : AbstractFacility
    {
        protected override void Init()
        {
            SQLiteConnection.CreateFile(Path.GetTempPath()+@"\StressData.s3db");
            NHConfigurator.CreateTestDbUsing(@"data source="+Path.GetTempPath()+@"\StressData.s3db; Version=3;");
            TestConnectionProvider.CloseDatabase();
            var cfg = NHConfigurator.Configuration;
            var schemaExport = new SchemaExport(cfg);
            schemaExport.Create(false, true);
            Kernel.Register(
                Component.For<ISessionFactory>()
                    .UsingFactoryMethod((kernel, context) => NHConfigurator.SessionFactory),
                Component.For<ISession>()
                    .UsingFactoryMethod((kernel, context) => kernel.Resolve<ISessionFactory>().OpenSession())
                    .LifestylePerWebRequest(),
                     Component.For<IStatelessSession>()
                    .UsingFactoryMethod((kernel, context) => kernel.Resolve<ISessionFactory>().OpenStatelessSession())
                    .LifestylePerWebRequest()
                );
        }
    }
}
