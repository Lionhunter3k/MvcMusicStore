using Autofac;
using CoreMusicStore.Infrastructure;
using CoreMusicStore.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
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
                    options.AdditionalCompilationReferences
                        .AddReferencesFromAssemblyOf<object>()
                        .AddReferencesFromAssemblyOf<Startup>()
                        .AddReferencesFromAssemblyOf<UrlResolutionTagHelper>()
                        .AddReferencesFromAssemblyOf<InputTagHelper>()
                        .AddReferencesFromAssemblyOf<HashSet<object>>()
                        .AddReferencesFromAssemblyOf<Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute>();
                })
                .AddDataAnnotations()
                .AddFormatterMappings()
                .AddJsonFormatters();

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
            });

            //otherwise, we should show a friendly error page that doesn't try to load too much stuff that might be broken
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
                  Path.Combine(Environment.WebRootPath, "Content")),
                RequestPath = "/Content"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Environment.WebRootPath, "Scripts")),
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
