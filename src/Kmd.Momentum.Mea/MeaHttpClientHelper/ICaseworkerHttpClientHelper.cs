using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Task.Model;
using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface ICaseworkerHttpClientHelper
    {
        Task<ResultOrHttpError<CaseworkerList, Error>> GetAllCaseworkerDataFromMomentumCoreAsync(string path, int pageNumber);

        Task<ResultOrHttpError<string, Error>> GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync(string path);
        Task<ResultOrHttpError<TaskList, Error>> GetAllTasksForCaseworkersAsync(string path, int pageNumber, string caseworkerId);
    }
}