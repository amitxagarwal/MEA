using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.HttpProvider
{
    public class Request
    {
        public string Term { get; set; }
        public Sort Sort { get; set; }
        public Paging Paging { get; set; }

    }


}