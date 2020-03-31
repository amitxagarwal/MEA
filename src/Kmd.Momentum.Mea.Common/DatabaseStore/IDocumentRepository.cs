using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface IDocumentRepository<TDocument> : IDocumentRepository<TDocument, Guid> where TDocument : IDocument
    {
    }

    public interface IDocumentRepository<TDocument, TKey> where TDocument : IDocument<TKey>
    {
        /// <summary>
        /// Insert or update a document
        /// </summary>
        /// <remarks>These methods ignore optimistic locking</remarks>
        /// <param name="entity">The document to upsert</param>
        Task<TDocument> SaveAsync(TDocument entity);
    }
}