using Kmd.Momentum.Mea.Common.DatabaseStore;
using Marten.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    [SoftDeleted]
    [UseOptimisticConcurrency]
    [AutoCreateDocumentCollection]
    [DocumentMappable("CitizenDataUpload")]
    [SuppressMessage("SonarAnalyzer.CSharp", "S107", Justification = "This is the base data for upload document and hence cannot avoid them.")]
    public class CitizenDataUploadModel : IDocument
    {
        [JsonProperty("id")]

        public Guid Id { get; }

        public string Name { get; }

        public string Address { get; }

        public int MobileNo { get; }

        public CitizenDataUploadModel(Guid id, string name, string address, int mobileNo)
        {
            Id = id;
            Name = name;
            Address = address;
            MobileNo = mobileNo;
        }
    }
}
