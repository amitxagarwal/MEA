using Marten.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoCreateDocumentCollectionAttribute : MartenAttribute
    {
        public override void Modify(DocumentMapping mapping)
        {
            mapping.Alias = DocumentStoreNames.EntityName(mapping.DocumentType);
        }
    }
}