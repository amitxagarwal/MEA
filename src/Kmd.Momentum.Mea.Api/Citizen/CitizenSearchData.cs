using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class CitizenSearchData
    {
        [JsonProperty("id")]
        public string CitizenId { get; }

        public string DisplayName { get; }
        //public string Description { get; }
        //public bool IsActive { get; }

        public CitizenSearchData(string id, string displayName)
        {
            CitizenId = id;
            DisplayName = displayName;
        }
    }
}
