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

            return new CitizenDataResponse(
                GetVal(json, "id"),
                GetVal(json, "displayName"),
                GetVal(json, "givenName"),
                GetVal(json, "middleName"),
                GetVal(json, "initials"),
                GetVal(json, "contactInformation.email.address"),
                GetVal(json, "contactInformation.phone.number"),
                GetVal(json, "caseworkerIdentifier"),
                GetVal(json, "description"));


        }

        public async Task<CitizenDataResponse> GetCitizenById(string citizenId)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCore(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}")).ConfigureAwait(false);
            var json = JObject.Parse(response);

            return new CitizenDataResponse(
                GetVal(json, "id"),
                GetVal(json, "displayName"),
                GetVal(json, "givenName"),
                GetVal(json, "middleName"),
                GetVal(json, "initials"),
                GetVal(json, "contactInformation.email.address"),
                GetVal(json, "contactInformation.phone.number"),
                GetVal(json, "caseworkerIdentifier"),
                GetVal(json, "description"));
        }

        private string GetVal(JObject _json, string _key)
        {
            string[] _keyArrr = _key.Split('.');
            var _subJson = _json[_keyArrr[0]];

            if (_subJson == null || String.IsNullOrEmpty(_subJson.ToString()))
                return String.Empty;

            if (_keyArrr.Length > 1)
            {
                _key = _key.Replace(_keyArrr[0] + ".", string.Empty, System.StringComparison.CurrentCulture);
                return GetVal((JObject)_subJson, _key);
            }
            return _subJson.ToString();
        }
    }
}
