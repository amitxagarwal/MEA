using Marten;
using Marten.Schema;
using Marten.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public class ScopedDocumentStore : IScopedDocumentStore
    {
        private readonly Marten.DocumentStore store;
        private bool disposedValue;
        private readonly List<IDisposable> registeredForDispose = new List<IDisposable>();
        private readonly ConcurrentDictionary<string, IQuerySession> querySessions = new ConcurrentDictionary<string, IQuerySession>();
        private readonly static ConcurrentDictionary<Type, TenancyStyle> mappedTenancy = new ConcurrentDictionary<Type, TenancyStyle>();

        public ScopedDocumentStore(Marten.DocumentStore store)
        {
            this.store = store;
        }

        public void AppendViews(string filePath)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            foreach (var mapping in store.Storage.AllDocumentMappings)
                sb.AppendLine(GenerateViewCommand(mapping));
            File.AppendAllText(filePath, sb.ToString());
        }

        public void WritePatch(string filename)
        {
            store.Schema.WritePatch(filename);
        }

        public static void ClearMappedTenancies()
        {
            mappedTenancy.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (disposing)
            {
                foreach (var item in registeredForDispose)
                {
                    item.Dispose();
                }
            }

            querySessions.Clear();
            disposedValue = true;
        }

        private static string GenerateViewCommand(IDocumentMapping mapping)
        {
            var selectFields = new List<string>();
            var tableFields = new List<string>();

            foreach (var prop in mapping.DocumentType.GetProperties())
            {
                var propType = prop.PropertyType;
                var propName = prop.Name;

                var jsonProp = prop.GetCustomAttributes(typeof(JsonPropertyAttribute)).FirstOrDefault() as JsonPropertyAttribute;
                if (!string.IsNullOrEmpty(jsonProp?.PropertyName))
                {
                    propName = jsonProp.PropertyName;
                }

                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propType = propType.GetGenericArguments().Single();
                }

                var propColumn = GetPropertyColumnType(propType);

                if (!string.IsNullOrEmpty(propColumn))
                {
                    tableFields.Add($"\"{propName}\" {propColumn}");
                    selectFields.Add($"x.\"{propName}\" as {ToSnakeCase(propName)}");
                }
            }

            if (tableFields.Count == 0) return null;

            var table = mapping.Table.Name;
            var selectList = string.Join(", ", selectFields);
            var tableList = string.Join(", ", tableFields);

            return $@"DROP VIEW IF EXISTS {table}_vw;
CREATE VIEW {table}_vw
AS SELECT {selectList} FROM {table}, jsonb_to_record(data) AS x({tableList});";
        }

        private static string ToSnakeCase(string str)
        {
            return string.Concat(
                str.Select(
                    (x, i) => i > 0 && char.IsUpper(x)
                    ? "_" + new string(char.ToLowerInvariant(x), 1)
                    : new string(char.ToLowerInvariant(x), 1)
                )
            );
        }

        private static string GetPropertyColumnType(Type propType)
        {
            if (columnMap.TryGetValue(propType, out string colType))
            {
                return colType;
            }

            if (propType.IsEnum)
            {
                return "text";
            }

            return null;
        }

        private readonly static Dictionary<Type, string> columnMap = new Dictionary<Type, string>
        {
            { typeof(string), "text" },
            { typeof(Guid), "uuid" },
            { typeof(short), "smallint" },
            { typeof(int), "int" },
            { typeof(long), "bigint" },
            { typeof(bool), "boolean" },
            { typeof(DateTime), "timestamp" },
            { typeof(DateTimeOffset), "timestamp with time zone" },
            { typeof(TimeSpan), "interval" },
            { typeof(double), "numeric" },
            { typeof(decimal), "numeric" },
            { typeof(float), "numeric" }
        };
    }
}