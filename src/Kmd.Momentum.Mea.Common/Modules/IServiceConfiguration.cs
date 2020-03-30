using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kmd.Momentum.Mea.Common.Modules
{
    public interface IServiceConfiguration
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}