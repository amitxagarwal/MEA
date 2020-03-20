using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kmd.Momentum.Mea;
using Kmd.Momentum.Mea.Api;

namespace Kmd.Momentum.Mea.Common.HttpProvider
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public class HttpClientHelper : IHttpClientHelper
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HttpClientHelper(IConfiguration config)
        {
            _httpClient = new HttpClient();
            _config = config;
        }

        private async Task<HttpResponseMessage> GetAuthorizationTokenAsync()
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

            var response = await _httpClient.PostAsync(new Uri($"{_config["Scope"]}"), content).ConfigureAwait(false);
            return response;
        }

        public async Task<IReadOnlyList<CitizenListResponse>> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var authResponse = await GetAuthorizationTokenAsync().ConfigureAwait(false);

            var accessToken = JObject.Parse(await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {(string)accessToken}");
            var sort = new Sort
            {
                FieldName = "cpr",
                Ascending = true
            };

            var paging = new Paging
            {
                PageNumber = -1,
                pageSize = 100
            };

            var req = new Request
            {
                Term = "25",
                Paging = paging,
                Sort = sort
            };

            bool hasMore = true;
            List<CitizenListResponse> citizenResponseList = new List<CitizenListResponse>();
            List<Task<string>> taskList = new List<Task<string>>();
            List<JToken> totalRecords = new List<JToken>();
            while (hasMore)
            {
                req.Paging.PageNumber += 1;
#pragma warning disable CA2000 // Dispose objects before losing scope
                var response = await _httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json")).ConfigureAwait(false);
#pragma warning restore CA2000 // Dispose objects before losing scope
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jsonArray = JArray.Parse(JObject.Parse(content)["data"].ToString());

                foreach (var record in jsonArray)
                {
                    var cpr = record["cpr"];

                    var task = GetFullDataMappingAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{cpr}"), record);
                    taskList.Add(task);
                }
                hasMore = (bool)JProperty.Parse(content)["hasMore"];

                totalRecords.AddRange(jsonArray.Children());
            }

            await Task.WhenAll(taskList);


            foreach (var task in taskList)
            {
                var result = task.Result;
                foreach (var jarr in totalRecords)
                {
                    if (JObject.Parse(result)["cpr"].ToString() == jarr["cpr"].ToString())
                    {
                        string address = string.Empty;
                        string number = string.Empty;

                        if (!String.IsNullOrEmpty(JObject.Parse(result)["contactInformation"].ToString()))
                        {
                            try
                            {
                                if (!String.IsNullOrEmpty(JObject.Parse(result)["contactInformation"]["email"].ToString()) &&
                                !String.IsNullOrEmpty(JObject.Parse(result)["contactInformation"]["email"]["address"].ToString()))
                                    address = JObject.Parse(result)["contactInformation"]["email"]["address"].ToString();

                                if (!String.IsNullOrEmpty(JObject.Parse(result)["contactInformation"]["phone"].ToString()) &&
                                !String.IsNullOrEmpty(JObject.Parse(result)["contactInformation"]["phone"]["number"].ToString()))
                                    number = JObject.Parse(result)["contactInformation"]["phone"]["number"].ToString();
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                        citizenResponseList.Add(new CitizenListResponse(jarr["citizenId"].ToString(),
                            jarr["cpr"].ToString(),
                            jarr["displayName"].ToString(),
                            address,
                            number));
                    }
                }
            }
            return citizenResponseList;
        }

        private async Task<string> GetFullDataMappingAsync(Uri url, JToken token)
        {
            var response = await GetDataAsync(url).ConfigureAwait(false);
            return response;
        }

        public async Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            var authResponse = await GetAuthorizationTokenAsync().ConfigureAwait(false);

            var accessToken = JObject.Parse(await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {(string)accessToken}");

            return await GetDataAsync(url).ConfigureAwait(false);

        }

        private async Task<string> GetDataAsync(Uri url)
        {
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            var citizenData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return citizenData;
        }

    }
}

