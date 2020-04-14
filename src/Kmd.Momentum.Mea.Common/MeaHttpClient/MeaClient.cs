using Kmd.Momentum.Mea.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.MeaHttpClient
{
    public class MeaClient : IMeaClient
    {
        private static HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _correlationId;

        public MeaClient(IConfiguration config, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpClient = httpClient;
            _correlationId = httpContextAccessor.HttpContext.TraceIdentifier;
        }
        public async Task<ResultOrHttpError<string, Error>> GetAsync(Uri url)
        {
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
                if ((int)response.StatusCode >= (int)HttpStatusCode.BadRequest && (int)response.StatusCode < (int)HttpStatusCode.InternalServerError)
                {
                    var errorFromResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var error = new Error(_correlationId, new string[] { "An error occured while fetching the record(s) from Core Api" }, "MEA");
                    Log.ForContext("CorrelationId", _correlationId).Error($"Error Occured while getting the data from Momentum Core System : {errorFromResponse}");

                    return new ResultOrHttpError<string, Error>(error, response.StatusCode);
                }

                var errorResponse = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                Log.ForContext("CorrelationId", _correlationId).Error($"Error Occured while getting the data from Momentum Core System {errorResponse}");
                return new ResultOrHttpError<string, Error>(errorResponse, response.StatusCode);
            }

            return new ResultOrHttpError<string, Error>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public async Task<ResultOrHttpError<string, Error>> PostAsync(Uri uri, StringContent stringContent)
        {
            var authResponse = await ReturnAuthorizationTokenAsync().ConfigureAwait(false);

            if (authResponse.IsError)
            {
                var error = new Error(_correlationId, new string[] { authResponse.Error }, "Momentum Core Api");
                return new ResultOrHttpError<string, Error>(error, authResponse.StatusCode.Value);
            }

            var accessToken = JObject.Parse(await authResponse.Result.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("bearer " + accessToken);

            var response = await _httpClient.PostAsync(uri, stringContent).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode >= (int)HttpStatusCode.BadRequest && (int)response.StatusCode < (int)HttpStatusCode.InternalServerError)
                {
                    var errorFromResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var error = new Error(_correlationId, new string[] { "An error occured while creating journal note in Momentum System" }, "MEA");
                    Log.ForContext("CorrelationId", _correlationId).Error($"Error Occured while creating journal note in Momentum System : {errorFromResponse}");

                    return new ResultOrHttpError<string, Error>(error, response.StatusCode);
                }

                var errorResponse = JsonConvert.DeserializeObject<Error>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                Log.ForContext("CorrelationId", _correlationId).Error($"Error Occured while creating journal note in Momentum System {errorResponse}");
                return new ResultOrHttpError<string, Error>(errorResponse, response.StatusCode);
            }

            return new ResultOrHttpError<string, Error>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        private async Task<ResultOrHttpError<HttpResponseMessage, string>> ReturnAuthorizationTokenAsync()
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
               {
                        new KeyValuePair<string, string>("grant_type","client_credentials"),
                        new KeyValuePair<string, string>("client_id", _config["KMD_MOMENTUM_MEA_McaClientId"]),
                        new KeyValuePair<string, string>("client_secret", _config["KMD_MOMENTUM_MEA_McaClientSecret"]),
                        new KeyValuePair<string, string>("resource", "74b4f45c-4e9b-4be1-98f1-ea876d9edd11")
               });

                var response = await _httpClient.PostAsync(new Uri($"{_config["Scope"]}"), content).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Log.ForContext("CorrelationId", _correlationId).Error($"Couldn't get the authorization from Momentum Core System with error as {errorResponse}", errorResponse);

                    return new ResultOrHttpError<HttpResponseMessage, string>("Current request is not authorized to access Momentum Core System", System.Net.HttpStatusCode.Unauthorized);
                }

                Log.ForContext("CorrelationId", _correlationId).Information("Current request is authorized to access Momentum Core System");
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