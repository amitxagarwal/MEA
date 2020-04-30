using Kmd.Momentum.Mea.Citizen.Model;
using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Tests.Citizen
{
    public class JournalNoteResponseBuilder
    {
        private string cpr = "testCpr";
        private string body = "testBody";
        private string title = "testTitle";
        private string type = "testType";
        private List<JournalNoteDocumentResponseModel> journalNoteDocumentResponseModel = new List<JournalNoteDocumentResponseModel>()
        {
            new JournalNoteDocumentResponseModel(){Content="testContent", ContentType="testContentType", Name="testDocumentName"}
        };

        public JournalNoteResponseModel Build()
        {
            return new JournalNoteResponseModel()
            {
                Cpr = this.cpr,
                Body = this.body,
                Title = this.title,
                Type = this.type,
                Documents = this.journalNoteDocumentResponseModel
            };
        }

        public JournalNoteResponseBuilder WithCpr(string cpr)
        {
            this.cpr = cpr;
            return this;
        }

        public JournalNoteResponseBuilder WithBody(string body)
        {
            this.body = body;
            return this;
        }

        public JournalNoteResponseBuilder WithTitle(string title)
        {
            this.title = title;
            return this;
        }

        public JournalNoteResponseBuilder WithDocuments(List<JournalNoteDocumentResponseModel> document)
        {
            this.journalNoteDocumentResponseModel = document;
            return this;
        }
    }
}
