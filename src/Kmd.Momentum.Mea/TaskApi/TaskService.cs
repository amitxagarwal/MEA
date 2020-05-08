using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Kmd.Momentum.Mea.TaskApi.Model;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.TaskApi
{
    public class TaskService : ITaskService
    {
        private readonly ITaskHttpClientHelper _taskHttpClient;
        private readonly string _correlationId;
        private readonly string _clientId;

        public TaskService(ITaskHttpClientHelper taskHttpClient, IHttpContextAccessor httpContextAccessor)
        {
            _taskHttpClient = taskHttpClient ?? throw new ArgumentNullException(nameof(taskHttpClient));
            _correlationId = httpContextAccessor.HttpContext.TraceIdentifier;
            _clientId = httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == "azp").Value;
        }

       public async Task<ResultOrHttpError<TaskDataResponseModel, Error>> UpdateTaskStatusByIdAsync(string taskId, TaskUpdateStatus taskUpdateStatus)
        {
            var taskStateValue = (int)taskUpdateStatus.TaskAction;
            var response = await _taskHttpClient.UpdateTaskStatusByTaskIdFromMomentumCoreAsync($"/tasks/{taskId}/{taskStateValue}?applicationContext={taskUpdateStatus.TaskContext}").ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("CorrelationId", _correlationId)
                   .ForContext("Client", _clientId)
                   .ForContext("TaskId", taskId)
                   .Error("An error occured while updating task state" + error);
                return new ResultOrHttpError<TaskDataResponseModel, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            var taskDataObj = JsonConvert.DeserializeObject<TaskData>(content);

            var dataToReturn = new TaskDataResponseModel(taskDataObj.Id, taskDataObj.Title, taskDataObj.Description, taskDataObj.Deadline, taskDataObj.CreatedAt,
               taskDataObj.StateChangedAt, taskDataObj.State, (IReadOnlyList<AssignedActors>)taskDataObj.AssignedActors, taskDataObj.Reference);

            Log.ForContext("CorrelationId", _correlationId)
                .ForContext("Client", _clientId)
                .ForContext("TaskId", taskDataObj.Id)
                .Information("The Task status is updated successfully");

            return new ResultOrHttpError<TaskDataResponseModel, Error>(dataToReturn);
        }
    }
}
