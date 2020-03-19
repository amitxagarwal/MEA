using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface IDocumentStoreAssemblyDiscoverer
    {
        IReadOnlyCollection<(Type type, AutoCreateDocumentCollectionAttribute attr)> DiscoverAutoDocumentCollectionTypes();

        IReadOnlyCollection<Type> DiscoverDocumentStoreConfigurers();
    }
}
