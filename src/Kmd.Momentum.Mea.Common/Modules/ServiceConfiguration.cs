using CorrelationId;
using Kmd.Momentum.Mea.Common.Authorization;
using Kmd.Momentum.Mea.Common.Framework;
using Kmd.Momentum.Mea.Common.Framework.PollyOptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kmd.Momentum.Mea.Common.Modules
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuthorizationHandler, MeaCustomClaimHandler>();
            services
                .AddPolicies(configuration)
                .AddHttpClient<IMeaClient, MeaClient, MeaClientOptions>(
                    configuration,
                    nameof(ApplicationOptions.MeaClient));

            services.AddCorrelationId();
        }
    }
}
