using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Kmd.Momentum.Mea.Api.Common
{
    public class CitizenHttpClient : ICitizenHttpClient
    {
        private readonly HttpClient _httpClient;
        public CitizenHttpClient()
        {
            _httpClient = new HttpClient();
        }       

        public async Task<string> ReturnAuthorizationToken()
        {
            var content = new FormUrlEncodedContent(new[]
           {
                        new KeyValuePair<string, string>("grant_type", "client_credentials"),
                        new KeyValuePair<string, string>("client_id", "4a7a4c73-f203-435e-b5c2-3cbba12f0285"),
                        new KeyValuePair<string, string>("client_secret", "a8FcVZ5gwaoHJf5TppvRCEN4wBWa?._-"),
                        new KeyValuePair<string, string>("resource", "74b4f45c-4e9b-4be1-98f1-ea876d9edd11")
                    });

            var response = await _httpClient.PostAsync(new Uri($"https://login.microsoftonline.com/momentumb2c.onmicrosoft.com/oauth2/token"),
                content).ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var json = JObject.Parse(responseBody);
            var accessToken = (string)json["access_token"];
            return accessToken;

        }

        public async Task<HttpResponseMessage> CallMCA(string httpMethod, Uri url, StringContent requestBody = null, HttpHeaders keyValuePairs = null)
        {
            switch(httpMethod)
            {
                case "get":

                    break;

                case "post":

                    break;


            }

        }
    }
}

