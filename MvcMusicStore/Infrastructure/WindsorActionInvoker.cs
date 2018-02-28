using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.Windsor;
using Castle.MicroKernel;
using System.Reflection;
using Castle.MicroKernel.ComponentActivator;
using System.Collections;

namespace MvcMusicStore.Infrastructure
{
    public class WindsorActionInvoker : ControllerActionInvoker
    {
        readonly IKernel container;

        public WindsorActionInvoker(IKernel container)
        {
            this.container = container;
        }

        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);
            var injectedFilters = new HashSet<object>();
            foreach (var filter in filters.ActionFilters)
            {
                if(injectedFilters.Add(filter))
                    container.InjectProperties(filter);
            }
            foreach (var filter in filters.AuthorizationFilters)
            {
                if (injectedFilters.Add(filter))
                    container.InjectProperties(filter);
            }
            foreach (var filter in filters.ExceptionFilters)
            {
                if (injectedFilters.Add(filter))
                    container.InjectProperties(filter);
            }
            foreach (var filter in filters.ResultFilters)
            {
                if (injectedFilters.Add(filter))
                    container.InjectProperties(filter);
            }
            return filters;
        }
    }

    public static class WindsorExtension
    {
        public static void InjectProperties(this IKernel kernel, object target)
        {
            var type = target.GetType();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite && kernel.HasComponent(property.PropertyType))
                {
                    var value = kernel.Resolve(property.PropertyType);
                    try
                    {
                        property.SetValue(target, value, null);
                    }
                    catch (Exception ex)
                    {
                        var message = string.Format("Error setting property {0} on type {1}, See inner exception for more information.", property.Name, type.FullName);
                        throw new ComponentActivatorException(message, ex, kernel.GetHandler(type).ComponentModel);
                    }
                }
            }
        }
    }
}
