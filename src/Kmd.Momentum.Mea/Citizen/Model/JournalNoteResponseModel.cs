using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Citizen.Model
{
    public class JournalNoteResponseModel
    {
        public string Cpr { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }
        
        public string Body { get; set; }

        public IReadOnlyList<JournalNoteDocumentResponseModel> Documents { get; set; }
    }
}