using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.SubSystems.Conversion;
using Common.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using System.Reflection;

namespace MvcMusicStore.Infrastructure.Logging
{
    public class CommonLoggingFacility : AbstractFacility
    {
        private ILoggerFactoryAdapter loggerFactory;
   
        protected override void Init()
        {
            if (loggerFactory == null)
            {
                loggerFactory = LogManager.Adapter;
            }
            Kernel.Register(Component.For<ILoggerFactoryAdapter>().NamedAutomatically("iloggerfactory").Instance(loggerFactory));
            Kernel.Register(Component.For<ILog>().NamedAutomatically("ilogger.default").Instance(loggerFactory.GetLogger("Default")));
            Kernel.Resolver.AddSubResolver(new LoggerResolver(loggerFactory));
        }
    }
}
