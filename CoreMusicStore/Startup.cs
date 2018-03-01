using Autofac;
using CoreMusicStore.Infrastructure;
using CoreMusicStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MvcMusicStore.Models;
using NHibernate.Dialect;
using NHibernate.Driver;
using System;
using System.Collections.Generic;
using System.IO;

namespace CoreMusicStore
{
    public class Startup : StartupBase
    {
        public IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment env)
        {
            Environment = env;
        }

        public override IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            return services.AddAutofacProvider(builder =>
            {
                builder.RegisterModule(new NHibernateModule<MsSql2008Dialect, SqlClientDriver>() { RootPath = Environment.ContentRootPath }.AddAssemblyFor<RegisteredUser>());
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddRouting();

            var mvcServiceBuilder = services
                .AddMvcCore()
                .AddApplicationPart<InputTagHelper>()
                .AddApplicationPart<UrlResolutionTagHelper>()
                .AddViews()
                .AddRazorViewEngine(options =>
                {
                    options.AddReferencesCore<object>();
                    options.AddReferencesCore<Startup>();
                    options.AddReferencesCore<UrlResolutionTagHelper>();
                    options.AddReferencesCore<InputTagHelper>();
                    options.AddReferencesCore<HashSet<object>>();
                    options.AddReferencesCore<Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute>();
                })
                .AddDataAnnotations()
                .AddFormatterMappings()
                .AddJsonFormatters();
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.MapWhen(context => context.Request.IsLocal(), localApp =>
            {
                localApp.UseDeveloperExceptionPage();
            });

            app.MapWhen(context => !context.Request.IsLocal(), remoteApp =>
            {
                app.UseExceptionHandler(exceptionApp =>
                {
                    exceptionApp.Use((context, next) =>
                    {
                        context.Request.Path = new PathString("/error.html");
                        return next();
                    });

                    exceptionApp.UseStaticFiles();
                });
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                  Path.Combine(Environment.ContentRootPath, "Content")),
                RequestPath = "/Content"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Environment.ContentRootPath, "Scripts")),
                RequestPath = "/Scripts"
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
