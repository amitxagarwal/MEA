using Marten;
using System;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface IScopedDocumentStore : IDisposable
    {
        /// <summary>
        /// Appends the views to the file
        /// </summary>
        void AppendViews(string filePath);

        /// <summary>
        /// Generates a patch file with latest schema changes
        /// </summary>
        void WritePatch(string filename);

        /// <summary>
        /// Get an update session.
        /// </summary>
        /// <typeparam name="T">The document type being managed</typeparam>
        /// <param name="tenant">The tenant to manage</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using (var session = store.LightweightSession<TDocument>(tenant))
        /// {
        ///     session.Store(entity);
        ///     await session.SaveChangesAsync().ConfigureAwait(false);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <returns>A lightweight (non-tracking) session.</returns>
        IDocumentSession LightweightSession<T>(string tenant = null);
    }
}