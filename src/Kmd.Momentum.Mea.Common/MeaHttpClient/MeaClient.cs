using Kmd.Momentum.Mea.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.MeaHttpClient
{
    public class MeaClient : IMeaClient
    {
        private static HttpClient _httpClient;
        private readonly IConfiguration _config;

        public MeaClient(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }
        public async Task<ResultOrHttpError<string, Error>> GetAsync(Uri url)
        {
            var authResponse = await ReturnAuthorizationTokenAsync().ConfigureAwait(false);

            var accessToken = JObject.Parse(await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("bearer " + accessToken);

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var errorResponse = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                if (errorResponse == null || errorResponse.Errors == null || errorResponse.Errors.Length <= 0)
                {
                    var error = new Error(Guid.NewGuid().ToString(), new string[] { "An error occured while fetching the record from Core Api" }, "MEA");
                    return new ResultOrHttpError<string, Error>(error, response.StatusCode);
                }

                return new ResultOrHttpError<string, Error>(errorResponse, response.StatusCode);
            }

            return new ResultOrHttpError<string, Error>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public async Task<ResultOrHttpError<string, Error>> PostAsync(Uri url, StringContent stringContent)
        {
            var authResponse = await ReturnAuthorizationTokenAsync().ConfigureAwait(false);

            var accessToken = JObject.Parse(await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("bearer " + accessToken);

            var response = await _httpClient.PostAsync(url, stringContent).ConfigureAwait(false);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var errorResponse = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                if (errorResponse == null || errorResponse.Errors == null || errorResponse.Errors.Length <= 0)
                {
                    var error = new Error(Guid.NewGuid().ToString(), new string[] { "An error occured while creating the record from Core Api" }, "MEA");
                    return new ResultOrHttpError<string, Error>(error, response.StatusCode);
                }

                return new ResultOrHttpError<string, Error>(errorResponse, response.StatusCode);
            }

            return new ResultOrHttpError<string, Error>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
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
    }
}