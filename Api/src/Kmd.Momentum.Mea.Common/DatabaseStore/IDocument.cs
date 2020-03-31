using System;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface IDocument : IDocument<Guid>
    {
    }

    public interface IDocument<out TKey> : IDocumentBase
    {
        TKey Id { get; }
    }

#pragma warning disable CA1040 // Avoid empty interfaces - intentional identifier
    public interface IDocumentBase
#pragma warning restore CA1040
    {

    }
}
