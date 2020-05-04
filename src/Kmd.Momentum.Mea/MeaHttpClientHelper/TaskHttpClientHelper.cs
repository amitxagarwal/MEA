//using Kmd.Momentum.Mea.Common.Exceptions;
//using Kmd.Momentum.Mea.Common.MeaHttpClient;
//using Kmd.Momentum.Mea.Task.Model;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace Kmd.Momentum.Mea.MeaHttpClientHelper
//{
//    public class TaskHttpClientHelper : ITaskHttpClientHelper
//    {
//        private readonly IMeaClient _meaClient;

//        public TaskHttpClientHelper(IMeaClient meaClient)
//        {
//            _meaClient = meaClient ?? throw new ArgumentNullException(nameof(meaClient));
//        }

//        public async Task<ResultOrHttpError<TaskList, Error>> GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync(string path, int pageNumber)
//        {
//            var pageSize = 100;
//            pageNumber = pageNumber == 0 ? 1 : pageNumber;
//            List<TaskDataResponseModel> totalRecords = new List<TaskDataResponseModel>();

//            //var queryStringParams = $"pagingInfo.pageNumber={pageNumber}&pagingInfo.pageSize={pageSize}";

//            //var response = await _meaClient.GetAsync(path + "?" + queryStringParams).ConfigureAwait(false);
//            TaskSearchQuery mcaRequestModel = new TaskSearchQuery()
//            {
//               AssignedActors = mcaRequestModel.
//            };

//            string serializedRequest = JsonConvert.SerializeObject(mcaRequestModel);
//            StringContent stringContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");


//            if (response.IsError)
//            {
//                return new ResultOrHttpError<TaskList, Error>(response.Error, response.StatusCode.Value);
//            }

//            var content = response.Result;
//            var caseworkerDataObj = JsonConvert.DeserializeObject<TaskFilter>(content);
//            var records = caseworkerDataObj.Data;

//            foreach (var item in records)
//            {
//                var dataToReturn = new TaskDataResponseModel(item.Id, item.Title, item.Description, item.Deadline, item.CreatedAt,
//               item.StateChangedAt);
//                totalRecords.Add(dataToReturn);
//            }

//            var responseData = new TaskList()
//            {
//                PageNo = pageNumber,
//                TotalNoOfPages = caseworkerDataObj.TotalPages,
//                TotalSearchCount = caseworkerDataObj.TotalSearchCount,
//                Result = totalRecords
//            };

//            return new ResultOrHttpError<TaskList, Error>(responseData);
//        }
//    }
//}
