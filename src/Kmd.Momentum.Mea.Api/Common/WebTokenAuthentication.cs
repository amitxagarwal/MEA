using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Common
{
    public class WebTokenAuthentication
    {
        public string GrantType { get; set; }
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public string Clientid { get; set; }
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public string ClientSecret { get; set; }
#pragma warning restore CA1707 // Identifiers should not contain underscores
        public string Resource { get; set; }
       
    }
}
