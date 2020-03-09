using Kmd.Momentum.Mea.Api.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    public class CitizenService : ICitizenService
    {
        private readonly ICitizenHttpClient _citizenHttpClient;

        public CitizenService(ICitizenHttpClient citizenHttpClient)
        {
            _citizenHttpClient = citizenHttpClient;
        }
        public async Task<int[]> GetAllActiveCitizens()
        {            
            await _citizenHttpClient.ReturnAuthorizationToken().ConfigureAwait(false);

            int[] citizens= new int[4] { 1, 2, 3, 4 };
            return citizens;
        }
    }
}
