using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Common
{
    public interface IHelperHttpClient
    {
        Task<string[]> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url);

        Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url);        
    }
}
