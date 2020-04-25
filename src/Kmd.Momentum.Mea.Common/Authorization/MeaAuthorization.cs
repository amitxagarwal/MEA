namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class MeaAuthorization
    {
        /// <summary>
        /// Kommunes of momentum core system
        /// </summary>     
        public string KommuneId { get; set; }

        /// <summary>
        /// KommuneUrl of momentum core system
        /// </summary>     
        public string KommuneUrl { get; set; }

        /// <summary>
        /// kommune client Id of momentum core system
        /// </summary>     
        public string KommuneClientId { get; set; }

        /// <summary>
        /// kommune client secret value identifier from key vault
        /// </summary>     
        public string KommuneAccessIdentifier { get; set; }

        /// <summary>
        /// kommune Resource Id for getting token
        /// </summary>     
        public string KommuneResource { get; set; }
    }
}
