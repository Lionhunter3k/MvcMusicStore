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
    public class XmlNhibernateModule : NHibernateModule
    {
        public string XmlCfgFileName { get; set; }

        protected override Configuration BuildConfiguration(IComponentContext context)
        {
            if (!string.IsNullOrEmpty(XmlCfgFileName))
            {
                var xmlConfigurationFilePath = XmlCfgFileName;
                if (!string.IsNullOrEmpty(SchemaRootPath))
                    xmlConfigurationFilePath = Path.Combine(SchemaRootPath, XmlCfgFileName);
                var nhConfig = new Configuration().Configure(xmlConfigurationFilePath);
                return nhConfig;
            }
            else
            {
                var nhConfig = new Configuration().Configure();
                return nhConfig;
            }
        }
    }
}
