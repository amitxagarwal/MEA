using Kmd.Momentum.Mea.Caseworker1.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Caseworker1
{
    public class CaseworkerService : ICaseworkerService
    {
        private readonly ICaseworkerHttpClientHelper _caseworkerHttpClient;
        private readonly IConfiguration _config;

        public CaseworkerService(ICaseworkerHttpClientHelper caseworkerHttpClient, IConfiguration config)
        {
            _caseworkerHttpClient = caseworkerHttpClient;
            _config = config;
        }

        public async Task<ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>> GetAllCaseworkersAsync()
        {
            var response = await _caseworkerHttpClient.GetAllCaseworkerDataFromMomentumCoreAsync
                (new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}/search")).ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("GetAllCaseworkersAsync", "All Caseworkers")
                .Error("An Error Occured while retriving data of all caseworkers" + error);
                return new ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>(response.Error, response.StatusCode.Value);
            }

            var result = response.Result;
            var content = result.Select(x => JsonConvert.DeserializeObject<CaseworkerDataResponseModel>(x));

            Log.ForContext("GetAllCaseworkersAsync", "All Caseworkers")
                .Information("All the caseworkers data retrived successfully");
            return new ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>(content.ToList());
        }

        public async Task<ResultOrHttpError<CaseworkerDataResponseModel, Error>> GetCaseworkerByIdAsync(string caseworkerId)
        {
            var caseworkerArr = await _caseworkerHttpClient.GetAllCaseworkerDataFromMomentumCoreAsync
                (new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}/search")).ConfigureAwait(false);
            var result = caseworkerArr.Result;
            var content = result.Select(x => JsonConvert.DeserializeObject<CaseworkerDataResponseModel>(x));
            //var a=JsonConvert.DeserializeObject<CaseworkerDataResponseModel[]>(caseworkerArr.Result.ToString());
            content.Where(caseworker => caseworker.CaseworkerId == caseworkerId);
            return new ResultOrHttpError<CaseworkerDataResponseModel, Error>(caseworkerArr.Error);
        }

        public async Task<IReadOnlyList<CaseworkerDataResponseModel>> GetCaseworkerIdAsync()
        {
            var response = await _caseworkerHttpClient.GetAllCaseworkerDatasFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7")).ConfigureAwait(false);

            return response;
        }


    }
}
    

