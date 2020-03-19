using Kmd.Momentum.Mea.Common.DatabaseStore;
using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Common.Modules
{
    public interface IMeaAssemblyDiscoverer : IDocumentStoreAssemblyDiscoverer
    {
        IReadOnlyCollection<IMeaOpenApiProduct> DiscoverOpenApiProducts();
    }
}
