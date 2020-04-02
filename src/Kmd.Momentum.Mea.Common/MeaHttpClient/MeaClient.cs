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
        private string _correlationId;

        public MeaClient(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }
        public async Task<ResultOrHttpError<string, Error>> GetAsync(Uri url)
        {

            _correlationId = _httpClient.DefaultRequestHeaders.GetValues("X-CORRELATION-ID").ToString();
            var authResponse = await ReturnAuthorizationTokenAsync().ConfigureAwait(false);

            if (authResponse.IsError)
            {
                var error = new Error(_correlationId, new string[] { authResponse.Error }, "Momentum Core Api");
                return new ResultOrHttpError<string, Error>(error, authResponse.StatusCode.Value);
            }

            var accessToken = JObject.Parse(await authResponse.Result.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("bearer " + accessToken);

            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                if (errorResponse == null || errorResponse.Errors == null || errorResponse.Errors.Length <= 0)
                {
                    var error = new Error(Guid.NewGuid().ToString(), new string[] { "An error occured while fetching the record from Core Api" }, "MEA");
                    Log.ForContext("CorrelationId", _correlationId).Error($"Error Occured while getting the data from Momentum Core System {error}");

                    return new ResultOrHttpError<string, Error>(error, response.StatusCode);
                }

                Log.ForContext("CorrelationId", _correlationId).Error($"Error Occured while getting the data from Momentum Core System {errorResponse}");
                return new ResultOrHttpError<string, Error>(errorResponse, response.StatusCode);
            }

            return new ResultOrHttpError<string, Error>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public Task<string> PostAsync(Uri uri, StringContent stringContent)
        {
            throw new NotImplementedException();
        }

        private async Task<ResultOrHttpError<HttpResponseMessage, string>> ReturnAuthorizationTokenAsync()
        {

            _correlationId = _httpClient.DefaultRequestHeaders.GetValues("X-CORRELATION-ID").ToString();
            try
            {
                var content = new FormUrlEncodedContent(new[]
               {
                        new KeyValuePair<string, string>("grant_type","client_credentials"),
                        new KeyValuePair<string, string>("client_id", _config["KMD_MOMENTUM_MEA_ClientId"]),
                        new KeyValuePair<string, string>("client_secret", _config["KMD_MOMENTUM_MEA_ClientSecret"]),
                        new KeyValuePair<string, string>("resource", "74b4f45c-4e9b-4be1-98f1-ea876d9edd11")
               });

                var response = await _httpClient.PostAsync(new Uri($"{_config["Scope"]}"), content).ConfigureAwait(false);

                if(!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Log.ForContext("CorrelationId", _correlationId).Error($"Couldn't get the authorization from Momentum Core System with error as {errorResponse}", errorResponse);

                    return new ResultOrHttpError<HttpResponseMessage, string>("Couldn't get the authorization to access Momentum Core System", System.Net.HttpStatusCode.Unauthorized);
                }

                Log.ForContext("CorrelationId", _correlationId).Information("Access granted to access Momentum Core System");
                return new ResultOrHttpError<HttpResponseMessage, string>(response);
            }
            catch (Exception ex)
            {
                Log.ForContext("CorrelationId", _correlationId).Error($"Couldn't fetch the configuration data to access Momentum Core System with error {ex.InnerException}");
                return new ResultOrHttpError<HttpResponseMessage, string>("Couldn't fetch the configuration data to access Momentum Core System", System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}