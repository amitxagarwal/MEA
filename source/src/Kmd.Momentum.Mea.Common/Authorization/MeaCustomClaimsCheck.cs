using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.Linq;

namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class MeaCustomClaimsCheck : IMeaCustomClaimsCheck
    {
        public MeaTokenClaimResponse FetchClaims(AuthorizationHandlerContext context, string audience, string tenant, string scope)
        {
            // If user does not have the audience, tenant and scope claim, get out of here
            if ((!context.User.HasClaim(c => c.Type == audience)) &&
                (!context.User.HasClaim(c => c.Type == tenant)) &&
                (!context.User.HasClaim(c => c.Type == scope)) &&
                (!context.User.HasClaim(c => c.Type == "azp")) is true)
            {
                Log.Error("The token to access the MEA does not contains all the relevant claims");

                return null;
            }

            var clientId = context.User.Claims.FirstOrDefault(x => x.Type == "azp").Value;

            Log.ForContext("ClientId", clientId)
                .Information("The token to access the MEA contains all the relevant claims");

            var tokenClaims = GetTokenClaims(context, clientId);

            if (tokenClaims != null)
            {
                return new MeaTokenClaimResponse(tokenClaims.Audience, tokenClaims.Tenant, tokenClaims.Scope);
            }

            return null;
        }

        private MeaTokenClaimResponse GetTokenClaims(AuthorizationHandlerContext context, string clientId)
        {
            // Split the audience, tenants, scope string into an array
            var audience = context.User.FindFirst(c => c.Type == MeaCustomClaimAttributes.AudienceClaimTypeName).Value.Split(' ');
            var tenant = context.User.FindFirst(c => c.Type == MeaCustomClaimAttributes.TenantClaimTypeName).Value;
            var scope = context.User.Claims.FirstOrDefault(x => x.Type.Contains("scope")).Value.Split(' ');

            if (audience.Length == 0 || tenant == null || scope.Length == 0)
            {
                Log.ForContext("ClientId", clientId)
                    .Error("Could not fetch the value of the MEA token claims");
                return null;
            }

            Log.ForContext("Tenant", tenant)
                .ForContext("ClientId", clientId)
                .Information("The audience, tenant and scope values from the MEA token are fetched successfully");

            return new MeaTokenClaimResponse(audience, tenant, scope);
        }
    }
}
