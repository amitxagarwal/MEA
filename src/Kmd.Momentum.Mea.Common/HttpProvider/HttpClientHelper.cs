using Kmd.Momentum.Mea.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.HttpProvider
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public class HttpClientHelper : IHttpClientHelper
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
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
#pragma warning disable CA2000 // Dispose objects before losing scope
            var content = new FormUrlEncodedContent(new[]
           {
#pragma warning disable CA1062 // Validate arguments of public methods
                        new KeyValuePair<string, string>("grant_type","client_credentials"),
#pragma warning restore CA1062 // Validate arguments of public methods
                        new KeyValuePair<string, string>("client_id", _config["KMD_MOMENTUM_MEA_ClientId"]),
                        new KeyValuePair<string, string>("client_secret", _config["KMD_MOMENTUM_MEA_ClientSecret"]),
                        new KeyValuePair<string, string>("resource", "74b4f45c-4e9b-4be1-98f1-ea876d9edd11")
                    });
#pragma warning restore CA2000 // Dispose objects before losing scope

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

            Log.Information("The active citizen data from Momentum core are returned successfully");

            return JsonConvert.DeserializeObject<string[]>(json);
        }

        public async Task<ResultOrHttpError<string,bool>> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url)
        {
            var authResponse = await ReturnAuthorizationTokenAsync().ConfigureAwait(false);

            var accessToken = JObject.Parse(await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {(string)accessToken}");

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new ResultOrHttpError<string, bool>(true,response.StatusCode);
            }

            var citizenData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            Log.Information("The citizen details by CPR or CitizenId from Momentum core are returned successfully");
            
            return new ResultOrHttpError<string,bool>(citizenData);
        }
    }
}

