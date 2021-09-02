﻿using System.Threading.Tasks;
using Edelstein.Common.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Edelstein.App.Standalone
{
    internal static class Program
    {
        private static Task Main(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", true);
                    builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true);
                    builder.AddCommandLine(args);
                })
                .ConfigureLogging(logging =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .MinimumLevel.Debug()
                        .CreateLogger();

                    logging.ClearProviders();
                    logging.AddSerilog();
                })

                .ConfigureDataStore()
                .ConfigureCaching()
                .ConfigureMessaging()
                .ConfigureParser()
                .ConfigureScripting()

                .ConfigureServices((context, builder) =>
                {
                    builder.Configure<ProgramConfig>(context.Configuration.GetSection("Host"));
                    builder.AddHostedService<ProgramHost>();
                })
                .RunConsoleAsync();
    }
}
