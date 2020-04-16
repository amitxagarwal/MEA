namespace Kmd.Momentum.Mea.Common.Authorization
{
    public static class MeaCustomClaimAttributes
    {
        public const string CitizenRole = "citizen";
        public const string CaseworkerRole = "caseworker";
        public const string JournalRole = "journal";

        public const string AudienceClaimTypeName = "aud";
        public const string TenantClaimTypeName = "tenant";
        public const string ScopeClaimTypeName = "http://schemas.microsoft.com/identity/claims/scope";
    }
}
