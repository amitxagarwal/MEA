using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Kmd.Momentum.Mea.TaskApi.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public class TaskHttpClientHelper : ITaskHttpClientHelper
    {
        private readonly IMeaClient _meaClient;

        public TaskHttpClientHelper(IMeaClient meaClient)
        {
            _meaClient = meaClient ?? throw new ArgumentNullException(nameof(meaClient));
        }

        public async Task<ResultOrHttpError<string, Error>> UpdateTaskStatusFromMomentumCoreAsync(string path, string taskId, TaskUpdateModel taskUpdateStatus)
        {
            var Result = new TaskUpdateModel();

            string serializedRequest = JsonConvert.SerializeObject(Result);
            StringContent stringContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");

            var response = await _meaClient.PutAsync(path, stringContent).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<string, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            return new ResultOrHttpError<string, Error>(content);
        }
    }

    }

