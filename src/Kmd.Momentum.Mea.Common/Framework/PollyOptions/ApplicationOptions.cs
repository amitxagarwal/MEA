using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.Framework.PollyOptions
{
    public class ApplicationOptions
    {
        public PolicyOptions Policies { get; set; }

        public MeaClientOptions MeaClient { get; set; }
    }
}
