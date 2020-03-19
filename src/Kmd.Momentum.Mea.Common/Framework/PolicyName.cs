using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.Framework
{
    public static class PolicyName
    {
        public const string HttpTimeout = nameof(HttpTimeout);
        public const string HttpRetry = nameof(HttpRetry);
        public const string HttpCircuitBreaker = nameof(HttpCircuitBreaker);
    }
}
