using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.Authorization.Tasks
{
    public class MeaTaskClaimRequirement : IAuthorizationRequirement
    {
        public string TaskAudience { get; }
        public string TaskTenant { get; }
        public string TaskScope { get; }

        public MeaTaskClaimRequirement(string audience, string tenant, string scope)
        {
            TaskAudience = audience ?? throw new ArgumentNullException(nameof(audience));
            TaskTenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            TaskScope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
    }
}
