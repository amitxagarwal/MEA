using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    internal static class DocumentStoreNames
    {
        private readonly static ConcurrentDictionary<Type, string> _names = new ConcurrentDictionary<Type, string>();

        public static string EntityName<T>()
            where T : IDocumentBase
        {
            return EntityName(typeof(T));
        }

        public static string EntityName(Type type)
        {
            if (_names.TryGetValue(type, out string entityName)) return entityName;

            var attr = type.GetCustomAttribute<DocumentMappableAttribute>();
            if (attr != null) return attr.TypeName;

            entityName = type.Name;
            const string modelSuffix = "Model";

            if (entityName.EndsWith(modelSuffix, StringComparison.Ordinal) && entityName.Length > modelSuffix.Length)
            {
                entityName = entityName.Substring(0, entityName.Length - modelSuffix.Length);
            }

            _names.TryAdd(type, entityName);

            return entityName;
        }
    }
}


