using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Citizen.Model
{
    public class JournalNoteRequestModel
    {
        public string Cpr { get; set; }

        public string Title { get; set; }

        public JournalNoteType Type { get; set; }
        
        public string Body { get; set; }

        public IReadOnlyList<JournalNoteDocumentRequestModel>? Documents { get; set; }
    }
}