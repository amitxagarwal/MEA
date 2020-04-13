using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Caseworker
{
    public interface ICaseworkerService
    {
        Task<ResultOrHttpError<IReadOnlyList<CaseworkerDataResponse>, Error>> GetAllCaseworkersAsync();
    }
}
