using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kmd.Momentum.Mea.Common.Exceptions;
using System.Net;

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
                return new ResultOrHttpError<IReadOnlyList<CitizenDataResponseModel>, Error>(response.Error);
            }

            var result = response.Result;
            var content = result.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x));

            return new ResultOrHttpError<IReadOnlyList<CitizenDataResponseModel>, Error>(content.ToList());
        }

        public async Task<ResultOrHttpError<CitizenDataResponseModel, Error>> GetCitizenByCprAsync(string cpr)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync
                (new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{cpr}")).ConfigureAwait(false);

            if (response.IsError)
            {
                Log.ForContext("CPR", cpr)
                .Error("An Error Occured while retriving citizen data by cpr");
                return new ResultOrHttpError<CitizenDataResponseModel, Error>(response.Error, HttpStatusCode.BadRequest);
            }

            var json = JObject.Parse(response.Result);
            var citizenData = JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString());

            Log.ForContext("CPR", cpr)
                .Information("The citizen details by CPR number is returned successfully");

            return new ResultOrHttpError<CitizenDataResponseModel, Error> (citizenData);
        }

        public async Task<ResultOrHttpError<CitizenDataResponseModel, Error>> GetCitizenByIdAsync(string citizenId)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}")).ConfigureAwait(false);
            if (response.IsError)
            {
                Log.ForContext("CitizenId", citizenId)
                .Error("An Error Occured while retriving citizen data by citizenID");
                return new ResultOrHttpError<CitizenDataResponseModel, Error>(response.Error);
            }

            var json = JObject.Parse(response.Result);

            Log.ForContext("CitizenId", citizenId)
                .Information("The citizen details by CitizenId has been returned successfully");

            return new ResultOrHttpError<CitizenDataResponseModel, Error>(JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString()));
        }
    }
}