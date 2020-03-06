using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Caseworker
{
    public class WebTokenAuthentication
    {
        public string Grant_type { get; set; }
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public string Client_id { get; set; }
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1707 // Identifiers should not contain underscores
        public string Client_secret { get; set; }
#pragma warning restore CA1707 // Identifiers should not contain underscores
        public string Resource { get; set; }
       
    }
}
