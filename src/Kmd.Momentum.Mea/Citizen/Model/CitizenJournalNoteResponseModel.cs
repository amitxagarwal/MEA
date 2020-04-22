using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Citizen.Model
{
    public class CitizenJournalNoteResponseModel
    {
        public string Cpr { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }
        
        public string Body { get; set; }

        public IReadOnlyList<CitizenJournalNoteDocumentResponseModel> Documents { get; set; }
    }
}