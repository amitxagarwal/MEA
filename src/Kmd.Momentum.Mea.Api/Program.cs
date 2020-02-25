using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace Kmd.Momentum.Mea.Api
{
    public static class Program
    {
        public static string GetEnvironmentInstanceId(IConfiguration configuration) =>
            configuration.GetValue("EnvironmentInstanceId", defaultValue: Environment.MachineName);

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables(prefix: "Momentum_External_API_").AddCommandLine(args).Build();
            var consoleMinLevel = config.GetValue("ConsoleLoggingMinLevel", defaultValue: LogEventLevel.Debug);
            var aspnetCoreLevel = config.GetValue("AspNetCoreLevel", defaultValue: LogEventLevel.Information);
            var applicationName = typeof(Program).Assembly.GetName().Name;
            var slotName = config.GetValue("SlotName", defaultValue: "localdev");
            var environmentInstanceId = GetEnvironmentInstanceId(config);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", aspnetCoreLevel)
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("SlotName", slotName)
                .Enrich.WithProperty("EnvironmentInstanceId", environmentInstanceId)
                .WriteTo.Console(restrictedToMinimumLevel: consoleMinLevel)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");

                using var host = CreateConfigurableHostBuilder(args, config).Build();

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateConfigurableHostBuilder(string[] args, IConfiguration config) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureAppConfiguration(builder =>
                        {
                            if (config != null) builder.AddConfiguration(config);
                        })
                        .UseStartup<Startup>();
                });
    }
}