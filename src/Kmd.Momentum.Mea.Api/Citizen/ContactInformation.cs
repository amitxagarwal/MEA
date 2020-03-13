using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class ContactInformation
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public PrimaryAddress PrimaryAddress { get; set; }
        public string TemporaryAddress { get; set; }
    }
}
