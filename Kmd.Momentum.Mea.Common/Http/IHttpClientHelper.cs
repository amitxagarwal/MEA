using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.Http
{
    public interface IHttpClientHelper
    {
        Task<string[]> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url);

        Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url);        
    }
}
