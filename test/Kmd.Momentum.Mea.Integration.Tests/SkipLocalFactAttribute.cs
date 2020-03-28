using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var logicApiUrl = deployedConfigJson.GetValue<Uri>(localApiUriKey, defaultValue: null);
            if (logicApiUrl == null)
                throw new Exception($"Expected to find '{localApiUriKey}' in 'appsettings' for the current environment");

            var logicApiUrlHostName = logicApiUrl?.Host;
            var isLocalHost = localhost.Equals(logicApiUrlHostName, StringComparison.OrdinalIgnoreCase);
            if (isLocalHost)
                Skip = $"'{localApiUriKey}' is '{logicApiUrl}' (Host='{logicApiUrlHostName}') "
                    + $"and this test only runs when the Host is NOT '{localhost}'.";
        }
    }
}
