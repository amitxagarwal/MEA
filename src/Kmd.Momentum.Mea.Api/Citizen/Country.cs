using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class Country
    {
        public bool Active { get; set; }
        public int? Children { get; set; }
        public string Code { get; set; }
        public bool HasExternalId { get; set; }
        public string Name { get; set; }
    }
}
