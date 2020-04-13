namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class Authorization
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
        /// Scopes used to authorize to momentum core system
        /// </summary>     
        public Scope Scopes { get; set; }
    }
}
