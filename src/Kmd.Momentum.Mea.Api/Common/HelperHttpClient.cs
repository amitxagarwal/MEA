using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace Kmd.Momentum.Mea.Api.Common
{
    public class HelperHttpClient : IHelperHttpClient
    {
        private readonly HttpClient _httpClient;
        public HelperHttpClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> ReturnAuthorizationToken(IConfiguration _config)
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            var content = new FormUrlEncodedContent(new[]
           {
#pragma warning disable CA1062 // Validate arguments of public methods
                        new KeyValuePair<string, string>("grant_type", _config["grant_type"]),
#pragma warning restore CA1062 // Validate arguments of public methods
                        new KeyValuePair<string, string>("client_id", _config["client_id"]),
                        new KeyValuePair<string, string>("client_secret", _config["client_secret"]),
                        new KeyValuePair<string, string>("resource", _config["resource"])
                    });
#pragma warning restore CA2000 // Dispose objects before losing scope

            var response = await _httpClient.PostAsync(new Uri($"https://login.microsoftonline.com/momentumb2c.onmicrosoft.com/oauth2/token"),
                content).ConfigureAwait(false);
            return response;

        }

        public async Task<HttpResponseMessage> CallMCA(IConfiguration _config, Uri url, string httpMethod = "get", StringContent requestBody = null)
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

