using Kmd.Momentum.Mea.Common.DatabaseStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public class LogicAssemblyDiscoverer : DocumentStoreAssemblyDiscoverer, ILogicAssemblyDiscoverer
    {
        private readonly List<(Assembly assembly, string productPathName, string openApiProductName, Version apiVersion)> _openApiProducts;

        public LogicAssemblyDiscoverer(IReadOnlyCollection<(Assembly assembly, string productPathName, string openApiProductName, Version apiVersion)> logicParts) :
            base(logicParts.Select(p => p.assembly))
        {
            _openApiProducts = logicParts.ToList();
        }

        public IReadOnlyCollection<ILogicOpenApiProduct> DiscoverOpenApiProducts()
        {
            return _openApiProducts
                .Select(t => new { t.productPathName, t.openApiProductName, t.apiVersion, t.assembly })
                .GroupBy(x => new { x.openApiProductName, x.productPathName })
                .Select(g =>
                {
                    var assemblies = g.Select(x => x.assembly).ToArray();
                    var apiVersion = g.Min(x => x.apiVersion);
                    return new LogicOpenApiProduct(g.Key.productPathName, g.Key.openApiProductName, apiVersion, assemblies);
                }).ToArray();
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