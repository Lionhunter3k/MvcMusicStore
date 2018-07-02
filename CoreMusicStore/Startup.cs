using Autofac;
using CoreMusicStore.Infrastructure;
using CoreMusicStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MvcMusicStore.Models;
using NHibernate.Dialect;
using NHibernate.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
                //nhibernate
                var nhibernateModule = new XmlNhibernateModule() { RootPath = Environment.ContentRootPath, XmlCfgFileName = "hibernate.cfg.mssql.xml" };
                nhibernateModule.SessionFactoryCreated += Seed.PopulateDatabase;
                builder.RegisterModule(nhibernateModule);

                //services
                builder.RegisterAssemblyTypes(typeof(Startup).Assembly)
                    .Where(c => (c.Namespace?.EndsWith("Components")).GetValueOrDefault() && c.GetInterfaces().Any(t => (t.Namespace?.EndsWith("Services")).GetValueOrDefault()))
                    .As(c => c.GetInterfaces().Where(t => (t.Namespace?.EndsWith("Services")).GetValueOrDefault()))
                    .PropertiesAutowired()
                    .InstancePerLifetimeScope();
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
                    options.AddReferencesFromRuntimeContext();
                })
                .AddDataAnnotations()
                .AddFormatterMappings()
                .AddJsonFormatters()
                .AddAuthorization();//this also calls services.AddAuthorization()

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(options =>
              {
                  // Cookie settings
                  options.Cookie.HttpOnly = true;
                  options.Cookie.Expiration = TimeSpan.FromDays(150);
                  options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                  options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                  // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                  options.SlidingExpiration = true;
              });
            //uncomment this line if we have a singleton which might be called from a HTTP request
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public override void Configure(IApplicationBuilder app)
        {
            //if we have an uncaught error, we should display the detailed error page if the request is local
            app.MapWhen(context => context.Request.IsLocal(), localApp =>
            {
                localApp.UseDeveloperExceptionPage();

                ConfigureApp(localApp);
            });

            //otherwise, we should show a friendly error page that doesn't try to load too much stuff that might be broken
            app.MapWhen(context => !context.Request.IsLocal(), remoteApp =>
            {
                remoteApp.UseExceptionHandler(exceptionApp =>
                {
                    exceptionApp.Use((context, next) =>
                    {
                        context.Request.Path = new PathString("/error.html");
                        return next();
                    });

                    exceptionApp.UseStaticFiles();
                });

                ConfigureApp(remoteApp);
            });
        }

        private void ConfigureApp(IApplicationBuilder app)
        {
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

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
