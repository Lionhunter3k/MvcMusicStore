using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.Core;
using Common.Logging;

namespace MvcMusicStore.Infrastructure
{
    public class LoggerSubDependencyResolver : ISubDependencyResolver
    {
        public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType == typeof(Common.Logging.ILog);
        }

        public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
        {
            if (CanResolve(context, contextHandlerResolver, model, dependency))
            {
                if (dependency.TargetItemType == typeof(Common.Logging.ILog))
                {
                    return Common.Logging.LogManager.GetLogger(model.Implementation);
                }
            }
            return null;
        }
    }
}
