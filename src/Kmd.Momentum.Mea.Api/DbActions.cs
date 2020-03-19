using Kmd.Momentum.Mea.Common.DatabaseStore;
using Serilog;
using System.IO;
using System.Text.RegularExpressions;

namespace Kmd.Momentum.Mea.Api
{
    public class DbActions
    {
        private readonly IScopedDocumentStore store;

        public DbActions(IScopedDocumentStore store)
        {
            this.store = store;
        }

        public int GenerateSchema(string filePath)
        {
            store.WritePatch(filePath);

            Log.Information("Auto correcting the generated script");
            string text = File.ReadAllText(filePath);
            text = Regex.Replace(text, ";;\r\n", ";\r\n");
            File.WriteAllText(filePath, text);
            Log.Information("Appending views");
            store.AppendViews(filePath);
            return 0;
        }
    }
}
