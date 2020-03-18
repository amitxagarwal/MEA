using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class CitizenSearchData
    {

        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("totalSearchCount")]
        public int TotalSearchCount { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

        [JsonProperty("data")]
        public IReadOnlyList<Data> Data { get; set; }



    }

    public class Data
    {
        [JsonProperty("citizenId")]
        public string CitizenId { get; set; }

        [JsonProperty("cpr")]
        public string Cpr { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }


}
