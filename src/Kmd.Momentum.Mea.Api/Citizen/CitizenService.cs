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
            HttpResponseMessage response = await _citizenHttpClient.GetMcaData(_config, new Uri($"{_config["McaUri"]}citizens/withActiveClassification"), "get").ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<string[]>(json);
            }
            throw new Exception(response.StatusCode.ToString());
        }

        public async Task<CitizenDataResponse> getCitizenByCpr(string cpr)
        {
            var response = await _citizenHttpClient.GetMcaData(_config, new Uri($"{_config["McaUri"]}citizens/{cpr}"), "get").ConfigureAwait(false);
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var data = JsonConvert.DeserializeObject<CitizenDataModel>(json);
            return new CitizenDataResponse(data.Id, true, data.DisplayName, null, null, null, data.ContactInformation.Email,
                data.ContactInformation.Phone, null, null, true);
        }

        public async Task<CitizenDataResponse> getCitizenById(string citizenId)
        {
            var url = await _citizenHttpClient.GetMcaData(_config, new Uri($"{_config["McaUri"]}citizens/{citizenId}"), "get").ConfigureAwait(false);
            var response = await url.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenDataModel>(response);
            return new CitizenDataResponse(actualResponse.Id, true, actualResponse.DisplayName, null, null, null, actualResponse.ContactInformation.Email,
                actualResponse.ContactInformation.Phone, null, null, true);
        }
    }
}
