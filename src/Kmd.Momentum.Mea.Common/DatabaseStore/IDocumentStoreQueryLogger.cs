using System;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface IDocumentStoreQueryLogger
    {
        IDocumentStoreQueryLoggerOperation BeginOperation(string operation);
    }

    public interface IDocumentStoreQueryLoggerOperation : IDisposable
    {
    }
}
