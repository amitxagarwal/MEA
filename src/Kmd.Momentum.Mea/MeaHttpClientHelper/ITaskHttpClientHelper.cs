using Kmd.Momentum.Mea.Common.Exceptions;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface ITaskHttpClientHelper
    {
        Task<ResultOrHttpError<string, Error>> UpdateTaskStatusByTaskIdFromMomentumCoreAsync(string path);
    }
}
