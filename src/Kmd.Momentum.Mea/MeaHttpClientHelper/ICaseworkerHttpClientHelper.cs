using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface ICaseworkerHttpClientHelper
    {
        Task<ResultOrHttpError<IReadOnlyList<CaseworkerDataResponse>, Error>> GetAllCaseworkerDataFromMomentumCoreAsync(Uri url);        
    }
}
