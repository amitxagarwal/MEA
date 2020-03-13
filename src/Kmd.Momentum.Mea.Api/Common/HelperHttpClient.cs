using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace Kmd.Momentum.Mea.Api.Common
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public class HelperHttpClient : IHelperHttpClient
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HelperHttpClient(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _config = config;
        }

        public async Task<HttpResponseMessage> ReturnAuthorizationToken(IConfiguration _config)
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

            var response = await _httpClient.PostAsync(new Uri($"{_config["Scope"]}"),
                content).ConfigureAwait(false);
            return response;

        }

        public async Task<HttpResponseMessage> GetMcaData(IConfiguration _config, Uri url, string httpMethod = "get", StringContent requestBody = null)
        {
            var authResponse = await ReturnAuthorizationToken(_config).ConfigureAwait(false);

            if (authResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return authResponse;
            }

            var accessToken = JObject.Parse(await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {(string)accessToken}");

            switch (httpMethod?.ToLower(CultureInfo.CurrentCulture))
            {
                case "get":
                    return await _httpClient.GetAsync(url).ConfigureAwait(false);

                case "post":
                    if (string.IsNullOrEmpty(requestBody?.ToString()))
                        return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

                    return await _httpClient.PostAsync(url, requestBody).ConfigureAwait(false);

                default: return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

            }
        }
    }
}

