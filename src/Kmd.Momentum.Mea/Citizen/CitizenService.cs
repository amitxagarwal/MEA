using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IReadOnlyList<CitizenDataResponseModel>> GetAllActiveCitizensAsync()
        {
            var response = await _citizenHttpClient.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}/search")).ConfigureAwait(false);

            var test = response.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x));
            return test.ToList();
        }

        public async Task<CitizenDataResponseModel> GetCitizenByCprAsync(string cpr)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{cpr}")).ConfigureAwait(false);
            var json = JObject.Parse(response);

            return JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString());
        }

        public async Task<CitizenDataResponseModel> GetCitizenByIdAsync(string citizenId)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}")).ConfigureAwait(false);
            var json = JObject.Parse(response);

            return JsonConvert.DeserializeObject<CitizenDataResponseModel>(json.ToString());
        }
    }
}