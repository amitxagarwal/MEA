using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.Authorization.Journal
{
    public class MeaJournalClaimHandler : AuthorizationHandler<MeaJournalClaimRequirement>
    {
        private readonly IConfiguration _configuration;
        private readonly IMeaCustomClaimsCheck _meaCustomClaimsCheck;

        public MeaJournalClaimHandler(IConfiguration configuration, IMeaCustomClaimsCheck meaCustomClaimsCheck)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _meaCustomClaimsCheck = meaCustomClaimsCheck ?? throw new ArgumentNullException(nameof(meaCustomClaimsCheck));
        }

        public const string Aud = "69d9693e-c4b7-4294-a29f-cddaebfa518b";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MeaJournalClaimRequirement requirement)
        {
            var claims = _meaCustomClaimsCheck.FetchClaims(context, requirement.JournalAudience, requirement.JournalTenant, requirement.JournalScope);

            if (claims != null && claims.Audience.Any(s => s == Aud) && CheckForValidScope(claims.Tenant, claims.Scope) is true)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool CheckForValidScope(string tenant, string[] scope)
        {
            bool result = false;
            var authorization = _configuration.GetSection("MeaAuthorization").Get<IReadOnlyList<MeaAuthorization>>().FirstOrDefault(x => x.KommuneId == tenant);
            var meaScope = _configuration.GetSection("MeaAuthorizationScopes:ScopeForJournalApi").Value;

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
