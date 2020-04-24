using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.Framework
{
    internal class UserAgentDelegatingHandler : DelegatingHandler
    {
        public const string UserAgentProductName = "Kmd.Momentum.Mea.Api";

        private static ProductInfoHeaderValue UserAgentValue { get; } = GetUserAgentValue();

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Headers.UserAgent.Any())
            {
                request.Headers.UserAgent.Add(UserAgentValue);
            }

            // Else the header has already been added due to a retry.

            return base.SendAsync(request, cancellationToken);
        }

        private static ProductInfoHeaderValue GetUserAgentValue()
        {
            var assembly = typeof(UserAgentDelegatingHandler).Assembly;

            var applicationVersion = GetVersion(assembly) ?? "1.0.0";

            return new ProductInfoHeaderValue(UserAgentProductName, applicationVersion);
        }

        private static string GetVersion(Assembly assembly)
        {
            var infoVersion = GetAttributeValue<AssemblyInformationalVersionAttribute>(assembly);
            if (infoVersion != null)
            {
                return infoVersion;
            }

            return GetAttributeValue<AssemblyFileVersionAttribute>(assembly);
        }

        private static string GetAttributeValue<T>(Assembly assembly)
            where T : Attribute
        {
            var type = typeof(T);

            var attribute = assembly
                .CustomAttributes
                .Where(x => x.AttributeType == type)
                .Select(x => x.ConstructorArguments.FirstOrDefault())
                .FirstOrDefault();

            return attribute.Value?.ToString();
        }
    }
}
