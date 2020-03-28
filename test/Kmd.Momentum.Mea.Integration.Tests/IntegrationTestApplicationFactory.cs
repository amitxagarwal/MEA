using Kmd.Momentum.Mea.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Integration.Tests
{
    public class IntegrationTestApplicationFactory : WebApplicationFactory<Startup>
    {
        public static string GetEnvironmentName() =>
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";        

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var env = GetEnvironmentName();

            builder.ConfigureAppConfiguration(config =>
            {                
                var integrationConfig = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{env}.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                config.AddConfiguration(integrationConfig);
            });
        }
    }
}
