namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class Scope
    {
        /// <summary>
        /// Scope to access citizen api of momentum core system
        /// </summary>     
        public string ScopeForCitizenApi { get; set; }

        /// <summary>
        /// Scope to access caseworker api of momentum core system
        /// </summary>     
        public string ScopeForCaseworkerApi { get; set; }

        /// <summary>
        /// Scope to access journal api of momentum core system
        /// </summary>     
        public string ScopeForJournalApi { get; set; }
    }
}
