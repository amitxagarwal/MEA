using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Task.Model;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Caseworker
{
    public interface ICaseworkerService
    {
        Task<ResultOrHttpError<CaseworkerList, Error>> GetAllCaseworkersAsync(int pagenumber);

        Task<ResultOrHttpError<CaseworkerDataResponseModel, Error>> GetCaseworkerByIdAsync(string id);

        Task<ResultOrHttpError<TaskList, Error>> GetAllTasksForCaseworkerIdAsync(string caseworkerId, int pagenumber);

    }
}
