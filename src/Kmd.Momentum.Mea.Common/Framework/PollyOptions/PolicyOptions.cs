using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.Framework.PollyOptions
{
    public class PolicyOptions
    {
        public CircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }

        public RetryPolicyOptions HttpRetry { get; set; }
    }
}
