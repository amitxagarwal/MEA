using Kmd.Momentum.Mea.Api.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var response = await _citizenHttpClient.GetAllActiveCitizenDataFromMomentumCore(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/withActiveClassification")).ConfigureAwait(false);
            return response;
        }

        public async Task<CitizenDataResponse> GetCitizenByCpr(string cpr)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCore(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{cpr}")).ConfigureAwait(false);
            var json = JObject.Parse(response);
            string email = string.Empty;
            string phone = string.Empty;

            if (!String.IsNullOrEmpty(json["contactInformation"]["email"].ToString()))
                email = json["contactInformation"]["email"]["address"].ToString();

            if (!String.IsNullOrEmpty(json["contactInformation"]["phone"].ToString()))
                phone = json["contactInformation"]["phone"]["number"].ToString();

            return new CitizenDataResponse(json["id"].ToString(),
            json["displayName"].ToString(),
            json["givenName"].ToString(),
            json["middleName"].ToString(),
            json["initials"].ToString(),
            email,
            phone,
            json["caseworkerIdentifier"].ToString(),
            json["description"].ToString());
        }

        public async Task<CitizenDataResponse> GetCitizenById(string citizenId)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCore(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}")).ConfigureAwait(false);

            return new CitizenDataResponse("",  "", null, null, null, "",
                "", null, null);
        }
    }
}
