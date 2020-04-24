using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
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
            var authorization = _configuration.GetSection("MeaAuthorization").Get<IReadOnlyList<Authorization>>().FirstOrDefault(x => x.KommuneId == tenant);

            if (authorization != null && scope.Contains(authorization.Scopes.ScopeForJournalApi))
                return true;

            return result;
        }
    }
}
