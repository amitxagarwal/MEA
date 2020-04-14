using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Citizen.Model
{
    public class McaCitizenJournalNoteRequestModel
    {
        public string Id { get; set; } //Map with CPR

        public string OccurredAt { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string Source { get; set; } //Mea

        public string ReferenceId { get; set; } //CitizenId

        public string JournalTypeId { get; set; } //sms(022.247.000) or other(022.420.000)

        public IReadOnlyList<McaCitizenJournalNoteRequestAttachmentModel> Attachments { get; set; }
    }
}
