using Kmd.Momentum.Mea.Citizen;
using Kmd.Momentum.Mea.Common.Modules;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Kmd.Momentum.Mea.Caseworker1;

namespace Kmd.Momentum.Mea.Modules
{
    public class ServiceConfiguration : IServiceConfiguration
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICitizenService, CitizenService>();
            services.AddScoped<ICitizenHttpClientHelper, CitizenHttpClientHelper>();

            services.AddScoped<ICaseworkerService, CaseworkerService>();
            services.AddScoped<ICaseworkerHttpClientHelper, CaseworkerHttpClientHelper>();
        }
    }
}
