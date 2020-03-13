using Kmd.Momentum.Mea.Api.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    public class CitizenService : ICitizenService
    {
        private readonly IHelperHttpClient _citizenHttpClient;
        private readonly IConfiguration _config;

        public CitizenService(IHelperHttpClient citizenHttpClient, IConfiguration config)
        {
            _citizenHttpClient = citizenHttpClient;
            _config = config;
        }
        public async Task<string[]> GetAllActiveCitizens()
        {
            HttpResponseMessage response = await _citizenHttpClient.GetDataFromMomentumCore(_config, new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/withActiveClassification"), "get").ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<string[]>(json);
            }
            throw new Exception(response.StatusCode.ToString());
        }

        public async Task<CitizenDataResponse> GetCitizenByCpr(string cpr)
        {
            var response = await _citizenHttpClient.GetDataFromMomentumCore(_config, new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{cpr}"), "get").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var data = JsonConvert.DeserializeObject<CitizenDataModel>(json);
            return new CitizenDataResponse(data.Id, null, data.DisplayName, null, null, null, data.ContactInformation.Email,
                data.ContactInformation.Phone, null, null, null);
        }

        public async Task<CitizenDataResponse> GetCitizenById(string citizenId)
        {
            var url = await _citizenHttpClient.GetDataFromMomentumCore(_config, new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}"), "get").ConfigureAwait(false);
            var response = await url.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenDataModel>(response);
            return new CitizenDataResponse(actualResponse.Id, null, actualResponse.DisplayName, null, null, null, actualResponse.ContactInformation.Email,
                actualResponse.ContactInformation.Phone, null, null, null);
        }
    }
}
