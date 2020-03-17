using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class HasResourceHandler : AuthorizationHandler<HasResourceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasResourceRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == Resource.ResourceClaimTypeName))
                return Task.CompletedTask;

            // Split the scopes string into an array
            var scopes = context.User.FindFirst(c => c.Type == Resource.ResourceClaimTypeName).Value.Split(' ');

            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == requirement.Resource))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}