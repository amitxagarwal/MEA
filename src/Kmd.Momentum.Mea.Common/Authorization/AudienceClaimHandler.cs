using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class AudienceClaimHandler : AuthorizationHandler<AudienceClaimRequirement>
    {
        public const string Aud = "69d9693e-c4b7-4294-a29f-cddaebfa518b";
        public readonly List<string> Tenant = new List<string> { "159","189" };
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AudienceClaimRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == Audience.AudienceClaimTypeName))
                return Task.CompletedTask;

            // Split the scopes string into an array
            var audience = context.User.FindFirst(c => c.Type == Audience.AudienceClaimTypeName).Value.Split(' ');
            var tenant = context.User.FindFirst(c => c.Type == Audience.TenantClaimTypeName).Value.Split(' ');

            // Succeed if the scope array contains the required scope
            if (audience.Any(s => s == Aud) && Tenant.Any(x=>tenant.Contains(x)))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}