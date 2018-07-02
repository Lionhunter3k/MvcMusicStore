using Autofac;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Models;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreMusicStore.Infrastructure.Persistence
{
    public class LoquaciousNHibernateModule<TDialect, TDriver> : NHibernateModule
          where TDialect : Dialect
          where TDriver : IDriver
    {
        private readonly HashSet<Assembly> _assemblies = new HashSet<Assembly>();

        public string ConnectionStringName { get; set; } = "DefaultConnection";

        public List<string> AssemblyNames { get; set; } = new List<string>();

        public LoquaciousNHibernateModule<TDialect, TDriver> AddAssemblyFor<TObject>()
        {
            Assembly assembly = typeof(TObject).GetTypeInfo().Assembly;
            _assemblies.Add(assembly);
            return this;
        }

        protected override Configuration BuildConfiguration(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();
            var config = new Configuration().DataBaseIntegration(db =>
            {
                db.Dialect<TDialect>();
                db.ConnectionStringName = configuration.GetConnectionString(ConnectionStringName);
                db.Driver<TDriver>();
                db.BatchSize = 100;
            });
            foreach (var assembly in _assemblies)
            {
                config.AddAssembly(assembly);
            }
            foreach (var assembly in AssemblyNames.Distinct())
            {
                config.AddAssembly(assembly);
            }

            return config;
        }
    }
}
