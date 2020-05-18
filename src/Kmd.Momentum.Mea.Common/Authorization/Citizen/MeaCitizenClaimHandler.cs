using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.Authorization.Citizen
{
    public class MeaCitizenClaimHandler : AuthorizationHandler<MeaCitizenClaimRequirement>
    {
        private readonly IConfiguration _configuration;
        private readonly IMeaCustomClaimsCheck _meaCustomClaimsCheck;

        public MeaCitizenClaimHandler(IConfiguration configuration, IMeaCustomClaimsCheck meaCustomClaimsCheck)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _meaCustomClaimsCheck = meaCustomClaimsCheck ?? throw new ArgumentNullException(nameof(meaCustomClaimsCheck));
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MeaCitizenClaimRequirement requirement)
        {
            var claims = _meaCustomClaimsCheck.FetchClaims(context, requirement.CitizenAudience, requirement.CitizenTenant, requirement.CitizenScope);

            if (claims != null && CheckForValidScope(claims.Tenant, claims.Scope) is true)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool CheckForValidScope(string tenant, string[] scope)
        {
            bool result = false;
            var authorization = _configuration.GetSection("MeaAuthorization").Get<IReadOnlyList<MeaAuthorization>>().FirstOrDefault(x => x.KommuneId == tenant);
            var meaScope = _configuration.GetSection("MeaAuthorizationScopes:ScopeForCitizenApi").Value;

            if (authorization == null || meaScope == null)
            {
                Log.ForContext("KommuneId", tenant)
                    .Error("The mea authorization settings are missing from configuration file");

                return result;
            }

            if (tenant == authorization.KommuneId && scope.Any(x => x == meaScope))
                return true;

            Log.ForContext("KommuneId", tenant)
                .Error("The mea authorization settings do not match do not match with the token claims");

            return result;
        }
    }
}
