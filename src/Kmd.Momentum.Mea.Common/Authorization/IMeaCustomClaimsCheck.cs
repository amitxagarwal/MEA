using Microsoft.AspNetCore.Authorization;

namespace Kmd.Momentum.Mea.Common.Authorization
{
    public interface IMeaCustomClaimsCheck
    {
        MeaTokenClaimResponse FetchClaims(AuthorizationHandlerContext context, string audience, string tenant, string scope);
    }
}
