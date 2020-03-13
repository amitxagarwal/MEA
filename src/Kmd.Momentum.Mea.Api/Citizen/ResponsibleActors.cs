using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class ResponsibleActors
    {
        public string ActorId { get; set; }
        public int Role { get; set; }
        public int?[] ResponsibilityCodes { get; set; }
    }
}
