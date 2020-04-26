using Kmd.Momentum.Mea.Common.DatabaseStore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace Kmd.Momentum.Mea.Api
{
    /// <summary>
    /// Starting point for the API
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// GetEnvironmentInstanceId method to get the instance id from environment variables
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static string GetEnvironmentInstanceId(IConfiguration configuration) =>
            configuration.GetValue("EnvironmentInstanceId", defaultValue: Environment.MachineName);

        /// <summary>
        /// Main method to start the api process
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables(prefix: "KMD_MOMENTUM_MEA_").AddCommandLine(args).AddUserSecrets<Startup>().Build();
            var consoleMinLevel = config.GetValue("ConsoleLoggingMinLevel", defaultValue: LogEventLevel.Debug);
            var aspnetCoreLevel = config.GetValue("AspNetCoreLevel", defaultValue: LogEventLevel.Information);
            var seqServerUrl = config.GetValue("DiagnosticSeqServerUrl", defaultValue: "http://localhost:5341/");
            var seqApiKey = config.GetValue("DiagnosticSeqApiKey", defaultValue: "");
            var applicationName = typeof(Program).Assembly.GetName().Name;
            var slotName = config.GetValue("SlotName", defaultValue: "localdev");
            var environmentInstanceId = GetEnvironmentInstanceId(config);

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft.AspNetCore", aspnetCoreLevel)
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithProperty("SlotName", slotName)
                .Enrich.WithProperty("EnvironmentInstanceId", environmentInstanceId)
                .WriteTo.Console(restrictedToMinimumLevel: consoleMinLevel)
                .WriteTo.Seq(serverUrl: seqServerUrl, apiKey: seqApiKey, compact: true)
#pragma warning disable CS0618 // Type or member is obsolete
                .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
#pragma warning restore CS0618 // Type or member is obsolete
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                using var host = CreateConfigurableHostBuilder(args, config).Build();

                var generateSchemaPath = config.GetValue<string>("generateschema");
                if (!string.IsNullOrEmpty(generateSchemaPath))
                {
                    Log.Information("Executing generateschema Command");
                    using (var scope = host.Services.CreateScope())
                    {
                        var store = scope.ServiceProvider.GetRequiredService<IScopedDocumentStore>();
                        var actions = new DbActions(store);
                        actions.GenerateSchema(generateSchemaPath);
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "A fatal exception was encountered");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        // NOTE: this exact signature is required by tooling such as the testing infrastructure
        /// <summary>
        /// Bulding the host for the API.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) => CreateConfigurableHostBuilder(args, config: null);

        /// <summary>
        /// Creating configurable host builder.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="config"></param>
        /// <returns></returns>
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
