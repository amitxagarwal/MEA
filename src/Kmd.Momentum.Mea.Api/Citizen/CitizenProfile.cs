using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class CitizenProfile
    {
        public bool GuestAuthorityParticipationUnwantedByCitizen { get; set; }
        public string ParticipationUnwantedByCitizenUpdatedAt { get; set; }
        public bool GuestAuthorityParticipationUnwantedByGuestAuthority { get; set; }
        public string ParticipationUnwantedByGuestAuthorityUpdatedAt { get; set; }
    }
}
