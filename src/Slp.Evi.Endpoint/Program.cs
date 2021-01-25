using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slp.Evi.Storage.MsSql;
using System;

namespace Slp.Evi.Endpoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if(args.Length == 1 && args[0] == "--version")
            {
                PrintVersion();
            }
            else
            {
                CreateHostBuilder(args).Build().Run();
            }
        }

        private static void PrintVersion()
        {
            var eviAssembly = typeof(MsSqlEviStorage).Assembly;
            Console.WriteLine($"EVI version: {eviAssembly.GetName().Version}");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables("EVI_");
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
