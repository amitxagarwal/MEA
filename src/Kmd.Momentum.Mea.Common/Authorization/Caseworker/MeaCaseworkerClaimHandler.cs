using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.Authorization.Caseworker
{
    public class MeaCaseworkerClaimHandler : AuthorizationHandler<MeaCaseworkerClaimRequirement>
    {
        private readonly IConfiguration _configuration;

        public MeaCaseworkerClaimHandler(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public const string Aud = "69d9693e-c4b7-4294-a29f-cddaebfa518b";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MeaCaseworkerClaimRequirement requirement)
        {
            // If user does not have the audience claim, get out of here
            if (!context.User.HasClaim(c => c.Type == requirement.CaseworkerAudience))
                return Task.CompletedTask;

            // If user does not have the tenant claim, get out of here
            if (!context.User.HasClaim(c => c.Type == requirement.CaseworkerTenant))
                return Task.CompletedTask;

            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == requirement.CaseworkerScope))
                return Task.CompletedTask;

            // Split the audience, tenants, scope string into an array
            var audience = context.User.FindFirst(c => c.Type == MeaCaseworkerClaimAttributes.CaseworkerAudienceClaimTypeName).Value.Split(' ');
            var tenant = context.User.FindFirst(c => c.Type == MeaCaseworkerClaimAttributes.CaseworkerTenantClaimTypeName).Value.Split(' ');
            var scope = context.User.Claims.FirstOrDefault(x => x.Type.Contains("scope")).Value.Split(' ');

            // Succeed if the audience, tenant and scope array contains the required audience, tenant and scope
            if (audience.Any(s => s == Aud) && CheckForValidScope(tenant, scope) is true)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }

        private bool CheckForValidScope(string[] tenant, string[] scope)
        {
            bool result = false;
            var authorization = _configuration.GetSection("MeaAuthorization").Get<IReadOnlyList<Authorization>>();

            foreach (var item in authorization)
            {
                if (tenant.Any(x => x == item.KommuneId) && scope.Any(x => x == item.Scopes.ScopeForCaseworkerApi))
                {
                    result = true;
                }
            }
            return result;
        }
    }
}
