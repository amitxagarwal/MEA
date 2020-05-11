using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.TaskApi.Model;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface ICaseworkerHttpClientHelper
    {
        Task<ResultOrHttpError<CaseworkerList, Error>> GetAllCaseworkerDataFromMomentumCoreAsync(string path, int pageNumber);
        Task<ResultOrHttpError<string, Error>> GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync(string path);
        Task<ResultOrHttpError<TaskList, Error>> GetAllTasksByCaseworkerIdFromMomentumCoreAsync(string path, int pageNumber, string caseworkerId);
    }
}