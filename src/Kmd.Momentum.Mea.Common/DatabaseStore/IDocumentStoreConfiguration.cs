using Marten;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface IDocumentStoreConfiguration
    {
        void Configure(StoreOptions options);
    }
}