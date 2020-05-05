using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.TaskApi.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.TaskApi
{
    public interface ITaskService
    {
        Task<ResultOrHttpError<TaskDataResponseModel, Error>> UpdateTaskStatusByIdAsync(string id, string taskUpdateStatus);
    }
}
