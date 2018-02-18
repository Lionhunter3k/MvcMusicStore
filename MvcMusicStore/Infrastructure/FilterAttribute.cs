using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.MicroKernel;

namespace MvcMusicStore.Infrastructure
{
    public interface IFilterFactory
    {
        AbstractFilter Resolve(Type type);

        void Release(AbstractFilter obj);
    }

    public class WindsorFilterFactory : IFilterFactory
    {
        private readonly IKernel kernel;

        public WindsorFilterFactory(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public AbstractFilter Resolve(Type type)
        {
            return (AbstractFilter)kernel.Resolve(type);
        }

        public void Release(AbstractFilter obj)
        {
            kernel.ReleaseComponent(obj);
        }
    }

    public abstract class AbstractFilter : IActionFilter,IResultFilter
    {
        public virtual void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public virtual void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public virtual void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        public virtual void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }
    }

    public class CustomFilterAttribute : ActionFilterAttribute
    {
        protected Type serviceType;

        public IFilterFactory Factory { get; set; }

        public Type ServiceType { get { return serviceType; } }
   
         public CustomFilterAttribute(Type serviceType) 
         {
             this.serviceType = serviceType;
         }

        public override void OnActionExecuting(ActionExecutingContext filterContext) 
        {
            var filter = Factory.Resolve(serviceType);
            try
            {
                filter.OnActionExecuting(filterContext);
            }
            finally
            {
                Factory.Release(filter);
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var filter = Factory.Resolve(serviceType);
            try
            {
                filter.OnActionExecuted(filterContext);
            }
            finally
            {
                Factory.Release(filter);
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var filter = Factory.Resolve(serviceType);
            try
            {
                filter.OnResultExecuted(filterContext);
            }
            finally
            {
                Factory.Release(filter);
            }
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var filter = Factory.Resolve(serviceType);
            try
            {
                filter.OnResultExecuting(filterContext);
            }
            finally
            {
                Factory.Release(filter);
            }
        }
        // alternatively swap c with some context defined by you
    }
}
