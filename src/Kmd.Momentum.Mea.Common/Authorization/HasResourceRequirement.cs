using Microsoft.AspNetCore.Authorization;
using System;

namespace Kmd.Momentum.Mea.Common.Authorization
{
    public class HasResourceRequirement : IAuthorizationRequirement
    {
        public string Resource { get; }

        public HasResourceRequirement(string resource)
        {
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        }
    }
}