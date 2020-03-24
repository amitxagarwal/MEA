using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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
            _httpClient.Timeout = new TimeSpan(0, 10, 10);
            _config = config;
        }

        private async Task GetAuthorizationTokenAsync()
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
            var accessToken = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false))["access_token"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {(string)accessToken}");
        }

        public async Task<IReadOnlyList<string>> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            await GetAuthorizationTokenAsync().ConfigureAwait(false);

            List<Task<string>> taskList = new List<Task<string>>();
            List<JToken> totalRecords = new List<JToken>();
            List<string> JsonStringList = new List<string>();

            var size = 15;
            var skip = 0;
            var totalNoOfRecords = 0;
            var remainingRecords = 0;

            do
            {
                var queryStringParams = $"term=Citizen&size={size}&skip={skip}&isActive=true";

#pragma warning disable CA2000 // Dispose objects before losing scope
                var response = await _httpClient.GetAsync(url + "?" + queryStringParams).ConfigureAwait(false);
#pragma warning restore CA2000 // Dispose objects before losing scope
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var jsonArray = JArray.Parse(JObject.Parse(content)["results"].ToString());

                totalNoOfRecords = (int)JProperty.Parse(content)["totalCount"];

                Parallel.ForEach(jsonArray, item =>
                {
                    var task = GetDataAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{item["id"]}"));
                    taskList.Add(task);
                });

                skip += size;

                remainingRecords = totalNoOfRecords - skip;

                totalRecords.AddRange(jsonArray.Children());
            } while (remainingRecords > 0);

            await Task.WhenAll(taskList);

            for (int i = 0; i < taskList.Count; i++)
            {
                var result = taskList[i].Result;
                var json = JObject.Parse(result);

                var jarr = totalRecords.Find(x => x["id"].ToString() == json["id"].ToString());

                if (jarr != null)
                {
                    var jsonToReturn = JsonConvert.SerializeObject(new
                    {
                        citizenId = jarr["id"],
                        displayName = jarr["name"],
                        givenName = "",
                        middleName = "",
                        initials = "",
                        address = GetVal(json, "contactInformation.email.address"),
                        number = GetVal(json, "contactInformation.phone.number"),
                        caseworkerIdentifier = "",
                        description = jarr["description"],
                        isBookable = true,
                        isActive = true
                    });
                    JsonStringList.Add(jsonToReturn);
                }
            }
            return JsonStringList;
        }

        public async Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            await GetAuthorizationTokenAsync().ConfigureAwait(false);

            var response = await GetDataAsync(url).ConfigureAwait(false);
            var json = JObject.Parse(response);

            return JsonConvert.SerializeObject(new
            {
                citizenId = GetVal(json, "id"),
                displayName = GetVal(json, "displayName"),
                givenName = GetVal(json, "givenName"),
                middleName = GetVal(json, "middleName"),
                initials = GetVal(json, "initials"),
                address = GetVal(json, "contactInformation.email.address"),
                number = GetVal(json, "contactInformation.phone.number"),
                caseworkerIdentifier = GetVal(json, "caseworkerIdentifier"),
                description = GetVal(json, "description"),
                isBookable = true,
                isActive = true
            });
        }

        private async Task<string> GetDataAsync(Uri url)
        {
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            var citizenData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return citizenData;
        }

        private string GetVal(JObject _json, string _key)
        {
            string[] _keyArr = _key.Split('.');
            var _subJson = _json[_keyArr[0]];

            if (_subJson == null || String.IsNullOrEmpty(_subJson.ToString()))
                return String.Empty;

            if (_keyArr.Length > 1)
            {
                _key = _key.Replace(_keyArr[0] + ".", string.Empty, System.StringComparison.CurrentCulture);
                return GetVal((JObject)_subJson, _key);
            }
            return _subJson.ToString();
        }
    }
}