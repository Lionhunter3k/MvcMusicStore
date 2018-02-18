using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;

namespace MvcMusicStore.Infrastructure
{
    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                                .BasedOn<IController>()
                                .LifestyleTransient(),
                                Classes.FromThisAssembly()
                                .BasedOn<AbstractFilter>()
                                .LifestyleTransient());
            container.Register(Component.For<IActionInvoker>().ImplementedBy<WindsorActionInvoker>().LifestylePerWebRequest());
            container.Register(Component.For<IFilterFactory>().ImplementedBy<WindsorFilterFactory>().LifestylePerWebRequest());
        }
    }
}
