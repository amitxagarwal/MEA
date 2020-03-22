using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface IHttpClientHelper
    {
        Task<string[]> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url);

        Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url);        
    }
}
