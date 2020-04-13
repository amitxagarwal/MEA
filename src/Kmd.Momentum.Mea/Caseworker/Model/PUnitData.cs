using Kmd.Momentum.Mea.Caseworker1.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Caseworker.Model
{
    public class PUnitData
    {
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("totalSearchCount")]
        public int TotalSearchCount { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

        [JsonProperty("additionalValues")]
        public string AdditionalValues { get; set; }

        [JsonProperty("data")]
        public CaseworkerDataResponseModel[] Data { get; set; }

    }
}

