using Kmd.Momentum.Mea.Common.DatabaseStore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface ILogicAssemblyDiscoverer : IDocumentStoreAssemblyDiscoverer
    {
        IReadOnlyCollection<ILogicOpenApiProduct> DiscoverOpenApiProducts();
    }
}
