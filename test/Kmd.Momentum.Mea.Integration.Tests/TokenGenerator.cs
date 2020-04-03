using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Integration.Tests
{
    public class TokenGenerator : ITokenGenerator
    {
        public async Task<string> GetToken()
        {
            var clientCrednetialsConfig = new ClientCredentialsConfig()
            {
                MeaClientId = Environment.GetEnvironmentVariable("MeaClientId"),
                MeaClientSecret = Environment.GetEnvironmentVariable("MeaClientSecret"),
                MeaScope = Environment.GetEnvironmentVariable("MeaScope"),
                Grant_Type = Environment.GetEnvironmentVariable("Grant_Type"),
                TokenEndPoint = "clientCredentials/token?issuer=b2clogin.com&tenant=159",
                TokenEndPointAddress = "https://identity-api.kmdlogic.io/clientCredentials/token?issuer=b2clogin.com&tenant=159"
            };
            
            var client = new HttpClient
            {
                BaseAddress = new Uri(clientCrednetialsConfig.TokenEndPointAddress)
            };

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientCrednetialsConfig.MeaClientId),
                new KeyValuePair<string, string>("client_secret", clientCrednetialsConfig.MeaClientSecret ),
                new KeyValuePair<string, string>("scope", clientCrednetialsConfig.MeaScope),
                new KeyValuePair<string, string>("grant_Type", clientCrednetialsConfig.Grant_Type)
            });

            var requestForToken = await client.PostAsync(clientCrednetialsConfig.TokenEndPointAddress, content);
            var result = await requestForToken.Content.ReadAsStringAsync();

            var json = JObject.Parse(result);
            var accessToken = (string)json["access_token"];

            return accessToken;
        }

        private class ClientCredentialsConfig
        {
            public string MeaClientId { get; set; }
            public string MeaClientSecret { get; set; }
            public string MeaScope { get; set; }
            public string Grant_Type { get; set; }
            public string  TokenEndPoint { get; set; }
            public string TokenEndPointAddress { get; set; }

        }
    }
}
