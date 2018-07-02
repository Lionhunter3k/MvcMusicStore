using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace CoreMusicStore.Infrastructure
{
    public static class MvcExtensions
    {
        public static IMvcCoreBuilder ConfigureActionInvokers(this IMvcCoreBuilder mvcServiceBuilder, Func<IServiceProvider, IActionInvokerFactory> actionInvokerFactory = null, Func<IServiceProvider, IActionInvokerProvider> actionInvokerProvider = null)
        {
            if(actionInvokerFactory == null)
                mvcServiceBuilder.Services.Replace(ServiceDescriptor.Singleton<IActionInvokerFactory, ActionInvokerFactory>());
            else
            {
                mvcServiceBuilder.Services.Replace(ServiceDescriptor.Singleton(actionInvokerFactory));
            }
            mvcServiceBuilder.Services.RemoveAll<IActionInvokerProvider>();
            if(actionInvokerProvider == null)
                mvcServiceBuilder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IActionInvokerProvider, ControllerActionInvokerProvider>());
            else
            {
                mvcServiceBuilder.Services.TryAddEnumerable(ServiceDescriptor.Transient(actionInvokerProvider));
            }
            return mvcServiceBuilder;
        }

        public static IMvcCoreBuilder ConfigureControllerFactories(this IMvcCoreBuilder mvcServiceBuilder,
            Func<IServiceProvider, IControllerActivatorProvider> controllerActivatorProvider = null,
            Func<IServiceProvider, IControllerActivator> controllerActivator = null,
            Func<IServiceProvider, IControllerFactoryProvider> controllerFactoryProvider = null,
            Func<IServiceProvider, IControllerFactory> controllerFactory = null)
        {
            if(controllerActivatorProvider == null)
                mvcServiceBuilder.Services.Replace(ServiceDescriptor.Singleton<IControllerActivatorProvider, ControllerActivatorProvider>());
            else
            {
                mvcServiceBuilder.Services.Replace(ServiceDescriptor.Singleton(controllerActivatorProvider));
            }
            if(controllerActivator == null)
                mvcServiceBuilder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, DefaultControllerActivator>());
            else
            {
                mvcServiceBuilder.Services.Replace(ServiceDescriptor.Transient(controllerActivator));
            }
            if(controllerFactoryProvider == null)
                mvcServiceBuilder.Services.Replace(ServiceDescriptor.Singleton<IControllerFactoryProvider, ControllerFactoryProvider>());
            else
            {
                mvcServiceBuilder.Services.Replace(ServiceDescriptor.Singleton(controllerFactoryProvider));
            }
            if(controllerFactory == null)
                mvcServiceBuilder.Services.Replace(ServiceDescriptor.Singleton<IControllerFactory, DefaultControllerFactory>());
            else
            {
                mvcServiceBuilder.Services.Replace(ServiceDescriptor.Singleton(controllerFactory));
            }
            //mvcServiceBuilder.AddControllersAsServices();
            //the line above does the following:
            //mvcServiceBuilder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            return mvcServiceBuilder;
        }

        public static IMvcCoreBuilder ConfigureFilterFactories(this IMvcCoreBuilder mvcServiceBuilder, Func<IServiceProvider, IFilterProvider> filterProvider = null)
        {
            mvcServiceBuilder.Services.RemoveAll<IFilterProvider>();
            if(filterProvider == null)
                mvcServiceBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IFilterProvider, DefaultFilterProvider>());
            else
            {
                mvcServiceBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(filterProvider));
            }
            return mvcServiceBuilder;
        }

        public static void AddReferencesFromRuntimeContext(this RazorViewEngineOptions options)
        {
            options.AdditionalCompilationReferences.AddReferencesFromRuntimeContext();
        }

        public static void AddReferencesFromRuntimeContext(this ICollection<MetadataReference> references)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            assemblies.Add(typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly);
            var refs = assemblies
                .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
                .Select(x => MetadataReference.CreateFromFile(x.Location))
                .ToList();
            foreach (var portableExecutableReference in refs)
            {
                references.Add(portableExecutableReference);
            }
        }

        public static unsafe void AddReferencesFromAssemblyOf<TObject>(this RazorViewEngineOptions options)
        {
            options.AddReferencesFromAssemblyOf<TObject>();
        }

        public static unsafe ICollection<MetadataReference> AddReferencesFromAssemblyOf<TObject>(this ICollection<MetadataReference> references)
        {
            Assembly assembly = typeof(TObject).GetTypeInfo().Assembly;
            // See http://www.strathweb.com/2016/03/roslyn-scripting-on-coreclr-net-cli-and-dnx-and-in-memory-assemblies/
            if (assembly.TryGetRawMetadata(out byte* b, out int length))
            {
                var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)b, length);
                var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                var result = assemblyMetadata.GetReference();
                references.Add(result);
                return references;
            }
            else
            {
                throw new Exception("Failed to get raw metadata for assembly: " + assembly.FullName);
            }
        }


        //mvcServiceBuilder.AddApplicationPart<InputTagHelper>();
        //mvcServiceBuilder.AddApplicationPart<UrlResolutionTagHelper>();
        public static IMvcCoreBuilder AddApplicationPart<TPart>(this IMvcCoreBuilder mvcServiceBuilder)
        {
            Assembly mvcTagHelpersAssembly = typeof(TPart).GetTypeInfo().Assembly;
            if (!Enumerable.OfType<AssemblyPart>(mvcServiceBuilder.PartManager.ApplicationParts).Any(p => p.Assembly == mvcTagHelpersAssembly))
                mvcServiceBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(mvcTagHelpersAssembly));
            return mvcServiceBuilder;
        }

        //mvcServiceBuilder.AddFeatureProviderPart<ViewComponentFeatureProvider>();
        //mvcServiceBuilder.AddFeatureProviderPart<MetadataReferenceFeatureProvider>();
        //mvcServiceBuilder.AddFeatureProviderPart<ViewsFeatureProvider>();
        public static IMvcCoreBuilder AddFeatureProviderPart<TPart>(this IMvcCoreBuilder mvcServiceBuilder)
            where TPart : IApplicationFeatureProvider, new()
        {
            Assembly mvcTagHelpersAssembly = typeof(TPart).GetTypeInfo().Assembly;
            if (!Enumerable.OfType<AssemblyPart>(mvcServiceBuilder.PartManager.FeatureProviders).Any(p => p.Assembly == mvcTagHelpersAssembly))
                mvcServiceBuilder.PartManager.FeatureProviders.Add(new TPart());
            return mvcServiceBuilder;
        }
    }
}
