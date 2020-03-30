using Newtonsoft.Json;
using System.Net;

namespace Kmd.Momentum.Mea.Common.Exceptions
{
    public class Error
    {
        [JsonProperty("correlationId")]
        public string CorrelationId { get; }

        [JsonProperty("errors")]
        public string[] Errors { get; }

        [JsonProperty("sourceSystem")]
        public string SourceSystem { get; }

        public Error(string correlationId, string[] errors, string sourceSystem)
        {
            CorrelationId = correlationId;
            Errors = errors;
            SourceSystem = sourceSystem;
        }
    }
}

