using Kmd.Momentum.Mea.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface ICaseworkerHttpClientHelper
    {
        Task<IReadOnlyList<CaseworkerDataResponseModel>> GetAllCaseworkerDataFromMomentumCoreAsync(Uri url);
              
    }
}
