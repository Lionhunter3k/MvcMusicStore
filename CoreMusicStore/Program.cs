using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CoreMusicStore
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();
                    if (args == null)
                        return;
                    config.AddCommandLine(args);
                })
                .UseDefaultServiceProvider((context, options) => options.ValidateScopes = true)
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
    }
}
