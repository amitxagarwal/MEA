using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Citizen
{
    public class CitizenService : ICitizenService
    {
        private readonly ICitizenHttpClientHelper _citizenHttpClient;
        private readonly IConfiguration _config;

        public CitizenService(ICitizenHttpClientHelper citizenHttpClient, IConfiguration config)
        {
            _citizenHttpClient = citizenHttpClient;
            _config = config;
        }

        public async Task<ResultOrHttpError<IReadOnlyList<CitizenDataResponseModel>, Error>> GetAllActiveCitizensAsync()
        {
            var response = await _citizenHttpClient.GetAllActiveCitizenDataFromMomentumCoreAsync
                (new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}/search")).ConfigureAwait(false);

            if(response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("GetAllActiveCitizensAsync", "All Active Citizens")
                .Error("An Error Occured while retriving data of all active citizens" + error);
                return new ResultOrHttpError<IReadOnlyList<CitizenDataResponseModel>, Error>(response.Error, response.StatusCode.Value);
            }

            var result = response.Result;
            var content = result.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x));

            Log.ForContext("GetAllActiveCitizensAsync", "All Active Citizens")
                .Information("All the active citizens data retrived successfully");
            return new ResultOrHttpError<IReadOnlyList<CitizenDataResponseModel>, Error>(content.ToList());
        }

        public async Task<ResultOrHttpError<CitizenDataResponseModel, Error>> GetCitizenByCprAsync(string cpr)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync
                (new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{cpr}")).ConfigureAwait(false);

            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("GetCitizenByCprAsync", "cpr")
                .Error("An Error Occured while retriving citizen data by cpr" + error);
                return new ResultOrHttpError<CitizenDataResponseModel, Error>(response.Error, response.StatusCode.Value);
            }

            var json = JObject.Parse(response.Result);
            var citizenData = JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString());

            Log.ForContext("GetCitizenByCprAsync", "cpr")
                .ForContext("CitizenId", citizenData.CitizenId)
                .Information("The citizen details by CPR number is returned successfully");

            return new ResultOrHttpError<CitizenDataResponseModel, Error> (citizenData);
        }

        public async Task<ResultOrHttpError<CitizenDataResponseModel, Error>> GetCitizenByIdAsync(string citizenId)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}")).ConfigureAwait(false);
            
            if (response.IsError)
            {
                var error = response.Error.Errors.Aggregate((a, b) => a + "," + b);
                Log.ForContext("GetCitizenByIdAsync", "citizenId")
                .ForContext("CitizenId", citizenId)
                .Error("An Error Occured while retriving citizen data by citizenID" + error);
                return new ResultOrHttpError<CitizenDataResponseModel, Error>(response.Error, response.StatusCode.Value);
            }

            var json = JObject.Parse(response.Result);
            var citizenData = JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString());

            Log.ForContext("GetCitizenByIdAsync", "citizenId")
                .ForContext("CitizenId", citizenData.CitizenId)
                .Information("The citizen details by CitizenId has been returned successfully");

            return new ResultOrHttpError<CitizenDataResponseModel, Error>(citizenData);
        }
    }
}