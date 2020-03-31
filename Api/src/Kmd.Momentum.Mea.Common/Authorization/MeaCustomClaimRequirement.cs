using Microsoft.AspNetCore.Authorization;
using System;

namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class MeaCustomClaimRequirement : IAuthorizationRequirement
    {
        public string Audience { get; }
        public string Tenant { get; }

        public MeaCustomClaimRequirement(string audience, string tenant)
        {
            Audience = audience ?? throw new ArgumentNullException(nameof(audience));
            Audience = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }
    }
}