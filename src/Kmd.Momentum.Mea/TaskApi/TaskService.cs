//using Kmd.Momentum.Mea.Common.Exceptions;
//using Kmd.Momentum.Mea.Task.Model;
//using Serilog;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace Kmd.Momentum.Mea.Task
//{
//    class TaskService : ITaskService
//    {
//        public async Task<ResultOrHttpError<TaskDataResponseModel, Error>> GetCaseworkerByIdAsync(string id)
//        {
//            var response = await _caseworkerHttpClient.GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync($"employees/{id}").ConfigureAwait(false);

//            if (response.IsError)
//            {
//                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
//                Log.ForContext("CorrelationId", _correlationId)
//                   .ForContext("Client", _clientId)
//                   .ForContext("CaseworkerId", id)
//                   .Error("An error occured while retrieving caseworker data by CaseworkerId" + error);
//                return new ResultOrHttpError<TaskDataResponseModel, Error>(response.Error, response.StatusCode.Value);
//            }

//            var content = response.Result;
//            var caseworkerDataObj = JsonConvert.DeserializeObject<TaskData>(content);

//            var dataToReturn = new TaskDataResponseModel(caseworkerDataObj.Id, caseworkerDataObj.DisplayName, caseworkerDataObj.GivenName,
//                caseworkerDataObj.MiddleName, caseworkerDataObj.Initials, caseworkerDataObj.Email?.Address, caseworkerDataObj.Phone?.Number, caseworkerDataObj.CaseworkerIdentifier,
//                caseworkerDataObj.Description, caseworkerDataObj.IsActive, caseworkerDataObj.IsBookable);

//            Log.ForContext("CorrelationId", _correlationId)
//                .ForContext("Client", _clientId)
//                .ForContext("CaseworkerId", caseworkerDataObj.Id)
//                .Information("The caseworker details by CaseworkerId has been returned successfully");

//            return new ResultOrHttpError<TaskDataResponseModel, Error>(dataToReturn);
//        }
//    }
//}
//}
