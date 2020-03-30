using System;
using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface IDocumentStoreAssemblyDiscoverer
    {
        IReadOnlyCollection<Type> DiscoverDocumentMappingTypes();

        IReadOnlyCollection<(Type type, AutoCreateDocumentCollectionAttribute attr)> DiscoverAutoDocumentCollectionTypes();

        IReadOnlyCollection<Type> DiscoverDocumentStoreConfigurers();
    }
}
