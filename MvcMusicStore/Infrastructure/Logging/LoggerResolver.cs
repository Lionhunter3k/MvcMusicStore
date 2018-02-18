using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel;
using Common.Logging;
using Castle.MicroKernel.Context;
using Castle.Core;
using System.Diagnostics;

namespace MvcMusicStore.Infrastructure.Logging
{
    public class LoggerResolver : ISubDependencyResolver
    {
        private readonly ILoggerFactoryAdapter loggerFactory;
        private readonly string logName;

        public LoggerResolver(ILoggerFactoryAdapter loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException("loggerFactory");
            }

            this.loggerFactory = loggerFactory;
        }


        public LoggerResolver(ILoggerFactoryAdapter loggerFactory, string name)
            : this(loggerFactory)
        {
            logName = name;
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType == typeof(ILog);
        }

        public object Resolve(CreationContext context, ISubDependencyResolver parentResolver, ComponentModel model, DependencyModel dependency)
        {
            Debug.Assert(CanResolve(context, parentResolver, model, dependency));
            Debug.Assert(loggerFactory != null);
            return string.IsNullOrEmpty(logName)
                ? loggerFactory.GetLogger(model.Implementation)
                : loggerFactory.GetLogger(logName);
        }
    }
}
