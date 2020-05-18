using Microsoft.AspNetCore.Authorization;
using System;

namespace Kmd.Momentum.Mea.Common.Authorization.Citizen
{
    public class MeaCitizenClaimRequirement : IAuthorizationRequirement
    {
        public string CitizenAudience { get; }
        public string CitizenTenant { get; }
        public string CitizenScope { get; }

        public MeaCitizenClaimRequirement(string audience, string tenant, string scope)
        {
            CitizenAudience = audience ?? throw new ArgumentNullException(nameof(audience));
            CitizenTenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            CitizenScope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
    }
}