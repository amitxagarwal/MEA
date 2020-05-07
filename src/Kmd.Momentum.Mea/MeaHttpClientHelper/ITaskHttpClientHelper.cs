using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.TaskApi.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface ITaskHttpClientHelper
    {
        Task<ResultOrHttpError<string, Error>> UpdateTaskStatusFromMomentumCoreAsync(string path, string taskId, TaskUpdateModel taskUpdateStatus);
    }
}
