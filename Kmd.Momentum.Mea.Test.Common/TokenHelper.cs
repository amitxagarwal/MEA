using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Test.Common
{
    public class TokenHelper : ITokenHelper
    {
        public async Task<string> GetToken()
        {
            var clientCredentialsConfiguration = new ClientCredentialsConfiguration
            {
                GrantType = "client_credentials",
                ClientId = "4a7a4c73-f203-435e-b5c2-3cbba12f0285",
                ClientSceret = "a8FcVZ5gwaoHJf5TppvRCEN4wBWa?._-",
                Resource = "74b4f45c-4e9b-4be1-98f1-ea876d9edd11",
                TokenEndPointBaseAddress = "https://kmd-rct-momentum-159-api.azurewebsites.net/",
                Scope = ""
            };

            var client = new HttpClient
            {
                BaseAddress = new Uri(clientCredentialsConfiguration.TokenEndPointBaseAddress)
            };

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", "4a7a4c73-f203-435e-b5c2-3cbba12f0285"),
                new KeyValuePair<string, string>("client_secret", "a8FcVZ5gwaoHJf5TppvRCEN4wBWa?._-"),
                new KeyValuePair<string, string>("resource", "74b4f45c-4e9b-4be1-98f1-ea876d9edd11")
            });

            var requestResult = await client.PostAsync(clientCredentialsConfiguration.TokenEndPoint, content);
            var contentResult = await requestResult.Content.ReadAsStringAsync();

            if (!requestResult.IsSuccessStatusCode)
            {
                throw new Exception(contentResult);
            }

            var json = JObject.Parse(contentResult);
            var accessToken = (string)json["access_token"];

            return accessToken;

        }

        private class ClientCredentialsConfiguration
        {
            public string GrantType { get; set; }
            public string ClientId { get; set; }
            public string ClientSceret { get; set; }
            public string Resource { get; set; }
            public string TokenEndPointBaseAddress { get; set; }
            public string TokenEndPoint { get; set; }
            public string Scope { get; set; }
        }
    }
}
