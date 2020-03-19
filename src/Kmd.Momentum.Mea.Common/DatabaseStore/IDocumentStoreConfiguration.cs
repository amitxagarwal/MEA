using Marten;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface IDocumentStoreConfiguration
    {
        void Configure(StoreOptions options);
    }
}