using Kmd.Momentum.Mea.Common.DatabaseStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kmd.Momentum.Mea.Common.Modules
{
    public class MeaAssemblyDiscoverer : DocumentStoreAssemblyDiscoverer, IMeaAssemblyDiscoverer
    {
        private readonly List<(Assembly assembly, string productPathName, string openApiProductName, Version apiVersion)> _openApiProducts;

        public MeaAssemblyDiscoverer(IReadOnlyCollection<(Assembly assembly, string productPathName, string openApiProductName, Version apiVersion)> meaParts) :
            base(meaParts.Select(p => p.assembly))
        {
            _openApiProducts = meaParts.ToList();
        }

        public IReadOnlyCollection<(Type type, AutoScopedDIAttribute attr)> DiscoverScopedDITypes() => Assemblies
               .SelectMany(x => x.GetTypes())
               .Select(x => (type: x, attr: x.GetCustomAttribute<AutoScopedDIAttribute>()))
               .Where(x => x.attr != null)
               .ToList();

        public IReadOnlyCollection<Type> DiscoverServiceConfigurers() => Assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IServiceConfiguration).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract && x.GetConstructor(Type.EmptyTypes) != null)
            .Select(x => x)
            .ToList();

        /// <summary>
        /// Discovers all concrete <see cref="IDocument"/> types in all assemblies. In other
        /// words, all document types which could be used for serialization, because they are
        /// creatable (classes which are not abstract).
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<Type> DiscoverConcreteDocumentTypes() => Assemblies
           .SelectMany(x => x.GetTypes())
           .Where(x => typeof(IDocumentBase).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
           .ToList();

        public void ConfigureDiscoveredServices(IServiceCollection services, IConfiguration configuration)
        {
            foreach (var cfgType in DiscoverServiceConfigurers())
            {
                ((IServiceConfiguration)Activator.CreateInstance(cfgType)).ConfigureServices(services, configuration);
            }
        }
    }
}