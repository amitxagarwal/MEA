using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.HttpProvider
{
    public interface IHttpClientHelper
    {
        Task<IReadOnlyList<string>> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url);

        Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url);        
    }
}
