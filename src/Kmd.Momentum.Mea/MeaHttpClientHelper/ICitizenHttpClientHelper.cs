using Kmd.Momentum.Mea.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface ICitizenHttpClientHelper
    {
        Task<ResultOrHttpError<IReadOnlyList<string>, bool>> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url);

        Task<ResultOrHttpError<string, bool>> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url);        
    }
}
