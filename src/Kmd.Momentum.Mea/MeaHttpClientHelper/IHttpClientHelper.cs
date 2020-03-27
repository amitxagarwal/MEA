using Kmd.Momentum.Mea.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface IHttpClientHelper
    {
        Task<ResultOrHttpError<string[], bool>> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url);

        Task<ResultOrHttpError<string, bool>> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url);        
    }
}
