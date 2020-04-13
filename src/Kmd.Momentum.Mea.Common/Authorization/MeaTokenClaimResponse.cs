using System;

namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class MeaTokenClaimResponse
    {
        public string[] Audience { get; }
        public string Tenant { get; }
        public string[] Scope { get; }

        public MeaTokenClaimResponse(string[] audience, string tenant, string[] scope)
        {
            Audience = audience ?? throw new ArgumentNullException(nameof(audience));
            Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
    }
}
