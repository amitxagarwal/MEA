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
        private readonly IHttpClientHelper _citizenHttpClient;
        private readonly IConfiguration _config;

        public CitizenService(IHttpClientHelper citizenHttpClient, IConfiguration config)
        {
            _citizenHttpClient = citizenHttpClient;
            _config = config;
        }

        public async Task<ResultOrHttpError<string[], bool>> GetAllActiveCitizensAsync()
        {
            var response = await _citizenHttpClient.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/withActiveClassification")).ConfigureAwait(false);

            if (response.IsError)
            {
                Log.Error("An Error Occured while retriving active citizens");
                return new ResultOrHttpError<string[], bool>(true, HttpStatusCode.NotFound);
            }

            return response;
        }

        public async Task<ResultOrHttpError<CitizenDataResponseModel, string>> GetCitizenByCprAsync(string cpr)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync
                (new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{cpr}")).ConfigureAwait(false);

            if (response.IsError)
            {
                Log.ForContext("CPR", cpr)
                .Error("An Error Occured while retriving citizen data by cpr");
                return new ResultOrHttpError<CitizenDataResponseModel, string>("An Error Occured while retriving citizen data by cpr", HttpStatusCode.NotFound);
            }

            var json = JObject.Parse(response.Result);

            Log.ForContext("CPR", cpr)
                .Information("The citizen details by CPR number is returned successfully", response.StatusCode);

            return JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString());
        }

        public async Task<ResultOrHttpError<CitizenDataResponseModel, string>> GetCitizenByIdAsync(string citizenId)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}")).ConfigureAwait(false);
            if (response.IsError)
            {
                Log.ForContext("CitizenId", citizenId)
                .Error("An Error Occured while retriving citizen data by citizenID");
                return new ResultOrHttpError<CitizenDataResponseModel, string>("An Error Occured while retriving citizen data by citizenId", HttpStatusCode.NotFound);
            }

            var json = JObject.Parse(response.Result);

            Log.ForContext("CitizenID", citizenId)
                .Information("The citizen details by CitizenId has been returned successfully", response.StatusCode);

            return JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString());
        }
    }
}