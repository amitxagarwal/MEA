﻿using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.HttpProvider;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
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

        public async Task<string[]> GetAllActiveCitizensAsync()
        {
            var response = await _citizenHttpClient.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/withActiveClassification")).ConfigureAwait(false);
            return response;
        }

        public async Task<CitizenDataResponseModel> GetCitizenByCprAsync(string cpr)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{cpr}")).ConfigureAwait(false);
            var json = JObject.Parse(response);

            return new CitizenDataResponseModel(
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

        public async Task<CitizenDataResponseModel> GetCitizenByIdAsync(string citizenId)
        {
            var response = await _citizenHttpClient.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}")).ConfigureAwait(false);
            var json = JObject.Parse(response);

            return new CitizenDataResponseModel(
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
            string[] _keyArr = _key.Split('.');
            var _subJson = _json[_keyArr[0]];

            if (_subJson == null || String.IsNullOrEmpty(_subJson.ToString()))
                return String.Empty;

            if (_keyArr.Length > 1)
            {
                _key = _key.Replace(_keyArr[0] + ".", string.Empty, System.StringComparison.CurrentCulture);
                return GetVal((JObject)_subJson, _key);
            }
            return _subJson.ToString();
        }
    }
}