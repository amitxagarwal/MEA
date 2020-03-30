using Kmd.Momentum.Mea.Common.DatabaseStore;
using Marten.Schema;
using Newtonsoft.Json;
using System;

namespace Kmd.Momentum.Mea.Citizen.Model
{
    [SoftDeleted]
    [UseOptimisticConcurrency]
    [AutoCreateDocumentCollection]
    [DocumentMappable("MeaPermission")]
    public class MeaPermissionModel : IDocument
    {
        [JsonProperty("id")]
        public Guid Id { get; }
        public int KommuneId { get; }
        public string ClientId { get; }
        public string Role { get; }

        public MeaPermissionModel(Guid id, int kommuneId, string clientId, string role)
        {
            Id = id;
            KommuneId = kommuneId;
            ClientId = clientId;
            Role = role;
        }
    }
}
