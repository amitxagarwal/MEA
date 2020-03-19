using Kmd.Momentum.Mea.Common.Authorization;
using Kmd.Momentum.Mea.Common.HttpProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kmd.Momentum.Mea.Common.Modules
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAuthorizationHandler, HasResourceHandler>();
            services.AddScoped<IHttpClientHelper, HttpClientHelper>();
        }
    }
}
