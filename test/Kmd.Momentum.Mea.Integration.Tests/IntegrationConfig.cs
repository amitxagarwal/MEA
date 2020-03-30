using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
