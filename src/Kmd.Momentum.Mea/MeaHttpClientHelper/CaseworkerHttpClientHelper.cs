using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Kmd.Momentum.Mea.TaskApi.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public class CaseworkerHttpClientHelper : ICaseworkerHttpClientHelper
    {
        private readonly IMeaClient _meaClient;

        public CaseworkerHttpClientHelper(IMeaClient meaClient)
        {
            _meaClient = meaClient ?? throw new ArgumentNullException(nameof(meaClient));
        }

        public async Task<ResultOrHttpError<CaseworkerList, Error>> GetAllCaseworkerDataFromMomentumCoreAsync(string path, int pageNumber)
        {
            var pageSize = 100;
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            List<CaseworkerDataResponseModel> totalRecords = new List<CaseworkerDataResponseModel>();

            var queryStringParams = $"pagingInfo.pageNumber={pageNumber}&pagingInfo.pageSize={pageSize}";

            var response = await _meaClient.GetAsync(path + "?" + queryStringParams).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<CaseworkerList, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            var caseworkerDataObj = JsonConvert.DeserializeObject<PUnitData>(content);
            var records = caseworkerDataObj.Data;

            foreach (var item in records)
            {
                var dataToReturn = new CaseworkerDataResponseModel(item.Id, item.DisplayName, item.GivenName, item.MiddleName, item.Initials,
               item.Email?.Address, item.Phone?.Number, item.CaseworkerIdentifier, item.Description, item.IsActive, item.IsBookable);
                totalRecords.Add(dataToReturn);
            }

            var responseData = new CaseworkerList()
            {
                PageNo = pageNumber,
                TotalNoOfPages = caseworkerDataObj.TotalPages,
                TotalSearchCount = caseworkerDataObj.TotalSearchCount,
                Result = totalRecords
            };

            return new ResultOrHttpError<CaseworkerList, Error>(responseData);
        }

        public async Task<ResultOrHttpError<string, Error>> GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync(string path)
        {
            var response = await _meaClient.GetAsync(path).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<string, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            return new ResultOrHttpError<string, Error>(content);
        }

        public async Task<ResultOrHttpError<TaskList, Error>> GetAllTasksByCaseworkerIdFromMomentumCoreAsync(string path, int pageNumber, string caseworkerId)
        {
            var pageSize = 100;
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            List<TaskDataResponseModel> totalRecords = new List<TaskDataResponseModel>();
            var Result = new TaskSearchQuery()
            {
                Start = null,
                End = null,
                AssignedActors = new List<string>() { caseworkerId },
                Types = new List<TaskType>() { },
                IsActive = true,
                IsCompleted = false,
                IsCanceled = false,
                IsStarted = true,
                IncludeUnassignedTasks = false
            };
            string serializedRequest = JsonConvert.SerializeObject(Result);
            StringContent stringContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

            var queryStringParams = $"pagingInfo.pageNumber={pageNumber}&pagingInfo.pageSize={pageSize}";

            var response = await _meaClient.PostAsync(path + "?" + queryStringParams, stringContent).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<TaskList, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            var caseworkerDataObj = JsonConvert.DeserializeObject<TaskFilter>(content);
            var records = caseworkerDataObj.Data;

            foreach (var item in records)
            {
                var dataToReturn = new TaskDataResponseModel(item.Id, item.Title, item.Description, item.Deadline, item.CreatedAt,
               item.StateChangedAt, item.State.ToString(), (IReadOnlyList<AssignedActors>)item.AssignedActors, item.Reference);
                totalRecords.Add(dataToReturn);
            }

            var responseData = new TaskList()
            {
                PageNo = pageNumber,
                TotalNoOfPages = caseworkerDataObj.TotalPages,
                TotalSearchCount = caseworkerDataObj.TotalSearchCount,
                Result = totalRecords
            };

            return new ResultOrHttpError<TaskList, Error>(responseData);
        }
    }
}