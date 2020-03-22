using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.Framework.PollyOptions
{
    public class HttpClientOptions
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}
