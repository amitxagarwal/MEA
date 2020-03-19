using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.HttpProvider
{
    public class HttpClientHelper : IHttpClientHelper
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HttpClientHelper(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _config = config;
        }

        private async Task<HttpResponseMessage> ReturnAuthorizationTokenAsync()
        {
            var content = new FormUrlEncodedContent(new[]
           {
                        new KeyValuePair<string, string>("grant_type","client_credentials"),
                        new KeyValuePair<string, string>("client_id", _config["KMD_MOMENTUM_MEA_ClientId"]),
                        new KeyValuePair<string, string>("client_secret", _config["KMD_MOMENTUM_MEA_ClientSecret"]),
                        new KeyValuePair<string, string>("resource", "74b4f45c-4e9b-4be1-98f1-ea876d9edd11")
                    });

            var response = await _httpClient.PostAsync(new Uri($"{_config["Scope"]}"), content).ConfigureAwait(false);
            return response;
        }

        public async Task<string[]> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url)
        {
            var authResponse = await ReturnAuthorizationTokenAsync().ConfigureAwait(false);

            var accessToken = JObject.Parse(await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {(string)accessToken}");

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<string[]>(json);
        }

        public async Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url)
        {
            var authResponse = await ReturnAuthorizationTokenAsync().ConfigureAwait(false);

            var accessToken = JObject.Parse(await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {(string)accessToken}");

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            var citizenData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return citizenData;
        }
    }
}

