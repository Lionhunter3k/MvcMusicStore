using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreMusicStore.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceProvider AddAutofacProvider(
     this IServiceCollection services,
     Action<ContainerBuilder> builderCallback = null)
        {
            // Instantiate the Autofac builder
            var builder = new ContainerBuilder();

            // If there is a callback use it for registrations
            builderCallback?.Invoke(builder);

            // Populate the Autofac container with services
            builder.Populate(services);

            // Create a new container with component registrations
            var container = builder.Build();

            // When application stops then dispose container
            container.Resolve<IApplicationLifetime>()
                .ApplicationStopped.Register(() => container.Dispose());

            // Return the provider
            return container.Resolve<IServiceProvider>();
        }
    }
}
