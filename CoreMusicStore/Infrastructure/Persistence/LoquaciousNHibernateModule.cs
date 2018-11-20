using Autofac;
using Microsoft.Extensions.Configuration;
using MvcMusicStore.Models;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
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
        public string AssemblyRootPath { get; set; }

        public Action<Configuration, IReadOnlyCollection<Type>, IReadOnlyCollection<Type>> OnModelCreating { get; set; } = CreateExplicitHbmMapping;

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
                db.ConnectionString = configuration.GetConnectionString(ConnectionStringName);
                db.Driver<TDriver>();
                db.BatchSize = 100;
            });
            foreach (var assembly in AssemblyNames.Distinct())
            {
                _assemblies.Add(Assembly.LoadFrom(AssemblyRootPath != null ? Path.Combine(AssemblyRootPath, assembly) : assembly));
            }
            foreach (var assembly in _assemblies)
            {
                config.AddAssembly(assembly);
            }
            var assemblyTypes = _assemblies.SelectMany(q => q.GetTypes()).ToList();
            var mappings = (from t in assemblyTypes
                            where t.BaseType != null && t.BaseType.IsGenericType
                            where typeof(IConformistHoldersProvider).IsAssignableFrom(t)
                            select t).ToList();
            OnModelCreating?.Invoke(config, mappings, assemblyTypes);
            return config;
        }

        private static void CreateExplicitHbmMapping(Configuration configuration, IReadOnlyCollection<Type> mappings, IReadOnlyCollection<Type> assemblyTypes)
        {
            if (mappings.Count > 0)
            {
                var mapper = new ModelMapper();
                mapper.AddMappings(mappings);
                var hbm = mapper.CompileMappingForAllExplicitlyAddedEntities();
                configuration.AddMapping(hbm);
            }
        }
    }
}
