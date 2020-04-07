using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Citizen.Model
{
    public class McaCitizenJournalNoteRequestModel
    {

        public string Cpr { get; set; }

        public string Title { get; set; }

        public string Email { get; set; }

        public string Source { get; set; }

        public string CreateDateTime { get; set; }

        public string Body { get; set; }

        public CitizenJournalNoteRequestDocumentModel[] Documents { get; set; }
    }
}
