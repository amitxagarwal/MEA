using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDocumentStore(this IServiceCollection services, IDocumentStoreAssemblyDiscoverer discoverer)
        {
            services.TryAddScoped<IScopedDocumentStore>(x => x.GetRequiredService<DocumentStoreFactory>().GetStore());
            services.TryAddSingleton<DocumentStoreFactory>();
            services.TryAddSingleton<DocumentMappableSerializationBinder>();
            services.TryAddSingleton<IDocumentStoreAssemblyDiscoverer>(discoverer);
            return services;
        }
    }
}
