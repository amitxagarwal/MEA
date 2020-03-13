using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class EnrollmentStatus
    {
        public DateTimeOffset? EndDate { get; set; }
        public int Status { get; set; }
    }
}
