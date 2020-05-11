using Kmd.Momentum.Mea.Citizen.Model;
using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Tests.Citizen
{
    public class JournalNoteResponseBuilder
    {
        private string cpr = "testCpr";
        private string body = "testBody";
        private string title = "testTitle";
        private JournalNoteType type = new JournalNoteType();
        private List<JournalNoteDocumentRequestModel> journalNoteDocumentRequestModel = new List<JournalNoteDocumentRequestModel>()
        {
            new JournalNoteDocumentRequestModel(){Content="testContent", ContentType="testContentType", Name="testDocumentName"}
        };

        public JournalNoteRequestModel Build()
        {
            return new JournalNoteRequestModel()
            {
                Cpr = this.cpr,
                Body = this.body,
                Title = this.title,
                Type = this.type,
                Documents = this.journalNoteDocumentRequestModel
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
        public JournalNoteResponseBuilder WithType(JournalNoteType type)
        {
            this.type = type;
            return this;
        }

        public JournalNoteResponseBuilder WithDocuments(List<JournalNoteDocumentRequestModel> document)
        {
            this.journalNoteDocumentRequestModel = document;
            return this;
        }
    }
}
