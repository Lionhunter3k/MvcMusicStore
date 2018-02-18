using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;
using MvcMusicStore.Models.Mappings;
using MvcMusicStore.Models.Mappings.Conventions;
using NHibernate.Tool.hbm2ddl;

namespace MvcMusicStore.Models.Tests.SessionFactoryBuilders
{
    public class SQLiteFluentSessionFactoryBuilder : ISessionFactoryBuilder
    {
        public ISessionFactory BuildSessionFactory()
        {
            FileHelper.DeletePreviousDbFiles();
            var dbFile = FileHelper.GetDbFileName();

            return Fluently.Configure()
                .Database(SQLiteConfiguration
                    .Standard.UsingFile(dbFile)
                    .ShowSql()
                    .FormatSql()
                    .AdoNetBatchSize(100))
                //.Cache(cache => cache.UseQueryCache().UseSecondLevelCache().ProviderClass("NHibernate.Cache.HashtableCacheProvider, NHibernate"))
                .Mappings(m => m.AutoMappings.Add(CreateAutomappings))
                .ExposeConfiguration(cfg => cfg.SetProperty("generate_statistics", "true"))
                .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, true))
                .BuildSessionFactory();
        }

        public static AutoPersistenceModel CreateAutomappings()
        {
            return AutoMap.AssemblyOf<JoinedRegisteredUser>(new AutomappingConfiguration())
                .Conventions.AddFromAssemblyOf<CustomForeignKeyConvention>()// c.Add<CustomForeignKeyConvention>())
                .UseOverridesFromAssemblyOf<JoinedRegisteredUser>();
        }
    }
}
