using System;
using System.Collections.Generic;
using System.Text;

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
    }
}