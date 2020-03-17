using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Common
{
    public interface IHelperHttpClient
    {
        Task<IReadOnlyList<CitizenSearchData>> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url);

        Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url);
    }
}
