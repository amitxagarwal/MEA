using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Caseworker
{
    public class CitizenHttpClient : ICitizenHttpClient
    {
        private readonly HttpClient _httpClient;
        public CitizenHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task ReturnAuthorizationToken()
        {
            WebTokenAuthentication abc = new WebTokenAuthentication()
            {
                Grant_type = "client_credentials",
                Client_id = "4a7a4373-f203-435e-b5c2-3cbba12f0285",
                Client_secret = "a8FcVZ5gwa0HJf5TppvRCEN4wBWa?._-",
                Resource = "74b4f45c-4e9b-4be1-98f1-ea876d9edd11"
            };
            _httpClient.BaseAddress = new Uri("");
            var requestBody = JsonConvert.SerializeObject(abc);

#pragma warning disable CA2000 // Dispose objects before losing scope
            var request = new HttpRequestMessage {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://login.microsoftonline.com/momentumb2c.onmicrosoft.com/oauth2/token"),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json"),
            };
#pragma warning restore CA2000 // Dispose objects before losing scope

            var response = _httpClient.SendAsync(request);
            return response;
           
        }
    }
}

