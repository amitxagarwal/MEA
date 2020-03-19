using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public class DocumentStoreAssemblyDiscoverer : IDocumentStoreAssemblyDiscoverer
    {
        public IReadOnlyCollection<Assembly> Assemblies { get; }

        public DocumentStoreAssemblyDiscoverer(IEnumerable<Assembly> assemblies)
        {
            Assemblies = assemblies.ToArray();
        }

        /// <summary>
        /// Returns the types decorated with the AutoCreateDocumentCollectionAttribute and the attribute itself
        /// </summary>
        public IReadOnlyCollection<(Type type, AutoCreateDocumentCollectionAttribute attr)> DiscoverAutoDocumentCollectionTypes() => Assemblies
           .SelectMany(x => x.GetTypes())
           .Select(x => (type: x, attr: x.GetCustomAttribute<AutoCreateDocumentCollectionAttribute>()))
           .Where(x => x.attr != null)
           .ToList();

        /// <summary>
        /// Discover all the classes responsible for describing the schema objects
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<Type> DiscoverDocumentStoreConfigurers() => Assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IDocumentStoreConfiguration).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract && x.GetConstructor(Type.EmptyTypes) != null)
            .Select(x => x)
            .ToList();
    }
}
