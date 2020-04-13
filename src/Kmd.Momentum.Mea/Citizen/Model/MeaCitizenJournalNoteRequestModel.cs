using Newtonsoft.Json;

namespace Kmd.Momentum.Mea.Citizen.Model
{
    public class MeaCitizenJournalNoteRequestModel
    {

        public string Cpr { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }
        
        public string Body { get; set; }

        public MeaCitizenJournalNoteRequestDocumentModel[] Documents { get; set; }
    }
}