using Kmd.Momentum.Mea.Common.Exceptions;
using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.HttpProvider
{
    public interface IHttpClientHelper
    {
        Task<string[]> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url);

        Task<ResultOrHttpError<string, bool>> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url);        
    }
}
