using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    internal class DocumentRepository<TDocument>
        : DocumentRepository<TDocument, Guid>, IDocumentRepository<TDocument>
        where TDocument : class, IDocument
    {
        public DocumentRepository(
            IScopedDocumentStore documentStore,
            IDocumentStoreQueryLogger queryLogger)
            : base(queryLogger, documentStore)
        {
        }
    }

    internal class DocumentRepository<TDocument, TKey> : IDocumentRepository<TDocument, TKey>
        where TDocument : class, IDocument<TKey>
    {
        private static readonly string _collectionName = DocumentStoreNames.EntityName(typeof(TDocument));

        private readonly IDocumentStoreQueryLogger _queryLogger;
        private readonly IScopedDocumentStore _documentStore;

        public DocumentRepository(IDocumentStoreQueryLogger queryLogger,
            IScopedDocumentStore documentStore)
        {
            _queryLogger = queryLogger ?? throw new ArgumentNullException(nameof(queryLogger));
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public Task<TDocument> SaveAsync(TDocument entity)
        {
            return SaveAsync(entity, null);
        }

        public async Task<TDocument> SaveAsync(TDocument entity, string tenant)
        {
            using (var oper = _queryLogger.BeginOperation($"Save to {_collectionName} with id {entity.Id}"))
            {
                using (var session = _documentStore.LightweightSession<TDocument>(tenant))
                {
                    session.Store(entity);

                    await session.SaveChangesAsync().ConfigureAwait(false);

                    return entity;
                }
            }
        }
    }
}
