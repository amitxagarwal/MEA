using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public class TaskHttpClientHelper : ITaskHttpClientHelper
    {
        private readonly IMeaClient _meaClient;

        public TaskHttpClientHelper(IMeaClient meaClient)
        {
            _meaClient = meaClient ?? throw new ArgumentNullException(nameof(meaClient));
        }

        public async Task<ResultOrHttpError<string, Error>> UpdateTaskStatusFromMomentumCoreAsync(string path)
        {
            var response = await _meaClient.PutAsync(path).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<string, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            return new ResultOrHttpError<string, Error>(content);
        }
    }

}

