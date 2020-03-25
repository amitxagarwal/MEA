using Kmd.Momentum.Mea.Citizen;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.DependencyInjection;

namespace Kmd.Momentum.Mea
{
    public static class MeaServiceCollectionExtensions
    {
        public static IServiceCollection AddMea(this IServiceCollection services)
        {
            services.AddScoped<IHttpClientHelper, HttpClientHelper>();
            services.AddScoped<ICitizenService, CitizenService>();
            return services;
        }
    }
}
