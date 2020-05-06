using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Kmd.Momentum.Mea.TaskApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.TaskApi
{
   public class TaskService : ITaskService
    {
        private readonly ITaskHttpClientHelper _taskHttpClient;
        private readonly IConfiguration _config;
        private readonly string _correlationId;
        private readonly string _clientId;

        public TaskService(ITaskHttpClientHelper taskHttpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _taskHttpClient = taskHttpClient ?? throw new ArgumentNullException(nameof(taskHttpClient));
            _config = config;
            _correlationId = httpContextAccessor.HttpContext.TraceIdentifier;
            _clientId = httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == "azp").Value;
        }
       public async Task<ResultOrHttpError<TaskDataResponseModel, Error>> UpdateTaskStatusByIdAsync(string taskID, TaskUpdateModel taskUpdateStatus)
        {
            var response = await _taskHttpClient.UpdateTaskStatusFromMomentumCoreAsync($"/tasks/{taskID}/{taskUpdateStatus}").ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("CorrelationId", _correlationId)
                   .ForContext("Client", _clientId)
                   .ForContext("CaseworkerId", taskID)
                   .Error("An error occured while retrieving caseworker data by CaseworkerId" + error);
                return new ResultOrHttpError<TaskDataResponseModel, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            var caseworkerDataObj = JsonConvert.DeserializeObject<TaskData>(content);

            var dataToReturn = new TaskDataResponseModel(caseworkerDataObj.Id, caseworkerDataObj.Title, caseworkerDataObj.Description, caseworkerDataObj.Deadline, caseworkerDataObj.CreatedAt,
               caseworkerDataObj.StateChangedAt, caseworkerDataObj.State, (IReadOnlyList<AssignedActors>)caseworkerDataObj.AssignedActors, caseworkerDataObj.Reference);

            Log.ForContext("CorrelationId", _correlationId)
                .ForContext("Client", _clientId)
                .ForContext("CaseworkerId", caseworkerDataObj.Id)
                .Information("The caseworker details by CaseworkerId has been returned successfully");

            return new ResultOrHttpError<TaskDataResponseModel, Error>(dataToReturn);
        }
    }
}
