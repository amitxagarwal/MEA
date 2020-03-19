using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class CitizenSearchData
    {
        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

        [JsonProperty("data")]
        public IReadOnlyList<CitizenListResponse> Data { get; set; }

    }

    public class CitizenListResponse
    {
        [JsonProperty("citizenId")]
        public string CitizenId { get; }

        [JsonProperty("cpr")]
        public string Cpr { get; }

        [JsonProperty("displayName")]
        public string DisplayName { get; }

        public CitizenListResponse(string citizenId, string cpr, string displayName)
        {
            CitizenId = citizenId;
            Cpr = cpr;
            DisplayName = displayName;
        }
    }


}
