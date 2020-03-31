using Microsoft.Extensions.Configuration;
using System;

namespace Kmd.Momentum.Mea.Integration.Tests
{
    public class IntegrationConfig
    {
        public static string GetEnvironmentName() =>
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        public static IConfigurationBuilder CreateConfigBuilder()
        {
            var env = GetEnvironmentName();

            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{env}.json");
            return builder;
        }
    }
}
