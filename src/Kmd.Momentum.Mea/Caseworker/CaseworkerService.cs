using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Caseworker
{
    public class CaseworkerService : ICaseworkerService
    {
        private readonly ICaseworkerHttpClientHelper _caseworkerHttpClient;
        private readonly IConfiguration _config;
        private readonly string _correlationId;
        private readonly string _clientId;

        public CaseworkerService(ICaseworkerHttpClientHelper caseworkerHttpClient, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _caseworkerHttpClient = caseworkerHttpClient ?? throw new ArgumentNullException(nameof(caseworkerHttpClient));
            _config = config;
            _correlationId = httpContextAccessor.HttpContext.TraceIdentifier;
            _clientId = httpContextAccessor.HttpContext.User.Claims.First(x => x.Type == "azp").Value;
        }

        public async Task<ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>> GetAllCaseworkersAsync()
        {
            var response = await _caseworkerHttpClient.GetAllCaseworkerDataFromMomentumCoreAsync
                (new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers")).ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);

                Log.ForContext("CorrelationId", _correlationId)
                   .ForContext("ClientId", _clientId)
                .Error("An Error Occured while retrieving data of all the caseworkers" + error);

                return new ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>(response.Error, response.StatusCode.Value);
            }

            Log.ForContext("CorrelationId", _correlationId)
               .ForContext("ClientId", _clientId)
                .Information("All the caseworkers data retrieved successfully");

            return new ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>(response.Result);
        }

       public async Task<ResultOrHttpError<CaseworkerDataResponseModel, Error>> GetCaseworkerByIdAsync(string caseworkerId)
        {
            var response = await _caseworkerHttpClient.GetCaseworkerDataByCaseworkerIdAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers") , caseworkerId).ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("Client", _clientId)
                 .ForContext("CitizenId", caseworkerId)
                .Error("An error occured while retrieving citizen data by citizenID" + error);
                return new ResultOrHttpError<CaseworkerDataResponseModel, Error>(response.Error, response.StatusCode.Value);
            }

            //var json = JObject.Parse(response.Result);
            //var citizenData = JsonConvert.DeserializeObject<CaseworkerDataResponseModel>(json.ToString());

            Log.ForContext("CorrelationId", _correlationId)
                    .ForContext("Client", _clientId)
                .ForContext("CitizenId", caseworkerId)
                .Information("The citizen details by CitizenId has been returned successfully");

            return new ResultOrHttpError<CaseworkerDataResponseModel, Error>(response.Result);
        }
    }
}


