using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace Kmd.Momentum.Mea.Integration.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SkipLocalFactAttribute: FactAttribute
    {
        private static IConfigurationRoot deployedConfigJson = IntegrationConfig.CreateConfigBuilder().Build();

        public SkipLocalFactAttribute()
        {
            const string localApiUriKey = "MeaUri";
            const string localhost = "localhost";
            var meaApiUrl = deployedConfigJson.GetValue<Uri>(localApiUriKey, defaultValue: null);
            if (meaApiUrl == null)
                throw new Exception($"Expected to find '{localApiUriKey}' in 'appsettings' for the current environment");

            var meaApiUrlHostName = meaApiUrl?.Host;
            var isLocalHost = localhost.Equals(meaApiUrlHostName, StringComparison.OrdinalIgnoreCase);
            if (isLocalHost)
                Skip = $"'{localApiUriKey}' is '{meaApiUrl}' (Host='{meaApiUrlHostName}') "
                    + $"and this test only runs when the Host is NOT '{localhost}'.";
        }
    }
}
