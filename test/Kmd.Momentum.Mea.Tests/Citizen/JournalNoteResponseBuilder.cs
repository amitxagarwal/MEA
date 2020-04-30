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
        //private IReadOnlyList<JournalNoteDocumentResponseModel> document;
        private string content = "testContent";
        private string contentType = "testContentType";
        private string name = "testDocumentName";


        public JournalNoteResponseModel Build()
        {
            return new JournalNoteResponseModel()
            {
                Cpr = this.cpr,
                Body = this.body,
                Title = this.title,
                Type = this.type
            };
        }

        public JournalNoteDocumentResponseModel Build1()
        {
            return new JournalNoteDocumentResponseModel()
            {
                Content = this.content,
                ContentType = this.contentType,
                Name = this.name
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

        //public JournalNoteResponseBuilder WithDocument(string document)
        //{
        //    this.document = document;
        //    return this;
        //}

    }
}
