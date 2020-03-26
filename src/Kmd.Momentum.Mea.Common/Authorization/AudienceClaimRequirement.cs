using Microsoft.AspNetCore.Authorization;
using System;

namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class AudienceClaimRequirement : IAuthorizationRequirement
    {
        public string Audience { get; }
        public string Tenant { get; }

        public AudienceClaimRequirement(string audience, string tenant)
        {
            Audience = audience ?? throw new ArgumentNullException(nameof(audience));
            Audience = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }
    }
}