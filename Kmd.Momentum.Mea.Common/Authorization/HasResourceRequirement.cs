using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;

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
