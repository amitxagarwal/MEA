using Microsoft.AspNetCore.Authorization;
using System;

namespace Kmd.Momentum.Mea.Common.Authorization.Caseworker
{
    public class MeaCaseworkerClaimRequirement : IAuthorizationRequirement
    {
        public string CaseworkerAudience { get; }
        public string CaseworkerTenant { get; }
        public string CaseworkerScope { get; }

        public MeaCaseworkerClaimRequirement(string audience, string tenant, string scope)
        {
            CaseworkerAudience = audience ?? throw new ArgumentNullException(nameof(audience));
            CaseworkerTenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            CaseworkerScope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
    }
}
