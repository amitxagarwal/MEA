using Microsoft.AspNetCore.Authorization;
using System;

namespace Kmd.Momentum.Mea.Common.Authorization.Journal
{
    public class MeaJournalClaimRequirement : IAuthorizationRequirement
    {
        public string JournalAudience { get; }
        public string JournalTenant { get; }
        public string JournalScope { get; }

        public MeaJournalClaimRequirement(string audience, string tenant, string scope)
        {
            JournalAudience = audience ?? throw new ArgumentNullException(nameof(audience));
            JournalTenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            JournalScope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
    }
}
