using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Integration.Tests
{
    public class TokenGenerator : ITokenGenerator
    {
        public static string GetMeaClientId() =>
            Environment.GetEnvironmentVariable("KMD_MOMENTUM_MEA_ClientId");

        public static string GetMeaClientSecret() =>
            Environment.GetEnvironmentVariable("KMD_MOMENTUM_MEA_ClientSecret");

        public static string GetMeaScope() =>
            Environment.GetEnvironmentVariable("KMD_MOMENTUM_MEA_Scope");

        public readonly string tokenEndPoint = "clientCredentials/token?issuer=b2clogin.com&tenant=159";

        public readonly string tokenEndPointAddress = "https://identity-api.kmdlogic.io/clientCredentials/token?issuer=b2clogin.com&tenant=159";

        public async Task<string> GetToken()
        {   
            var client = new HttpClient
            {
                BaseAddress = new Uri(tokenEndPointAddress)
            };

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", GetMeaClientId()),
                new KeyValuePair<string, string>("client_secret", GetMeaClientSecret()),
                new KeyValuePair<string, string>("scope", GetMeaScope()),
                new KeyValuePair<string, string>("grant_Type", "client_credentials")
            });

            var requestForToken = await client.PostAsync(tokenEndPointAddress, content);
            var result = await requestForToken.Content.ReadAsStringAsync();

            var json = JObject.Parse(result);
            var accessToken = (string)json["access_token"];

            return accessToken;
        }
    }
}
