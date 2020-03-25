using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public class HttpClientHelper : IHttpClientHelper
    {
        private readonly IMeaClient _meaClient;
        private readonly IConfiguration _config;

        public HttpClientHelper(IMeaClient meaClient,
            IConfiguration config)
        {
            _meaClient = meaClient;
            _config = config;
        }

        public async Task<IReadOnlyList<string>> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url)
        {
            List<Task<string>> taskList = new List<Task<string>>();
            List<JToken> totalRecords = new List<JToken>();
            List<string> JsonStringList = new List<string>();

            var size = 80;
            var skip = 0;
            var totalNoOfRecords = 0;
            var remainingRecords = 0;
            
            var authResponse = await _meaClient.ReturnAuthorizationTokenAsync().ConfigureAwait(false);
            var accessToken = await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            do
            {                
                var queryStringParams = $"term=Citizen&size={size}&skip={skip}&isActive=true";
                var content = await _meaClient.GetAsync(new Uri(url + "?" + queryStringParams), accessToken).ConfigureAwait(false);
                
                try
                {
                    var jsonArray = JArray.Parse(JObject.Parse(content)["results"].ToString());

                    totalNoOfRecords = (int)JProperty.Parse(content)["totalCount"];

                    Parallel.ForEach( totalRecords,new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount*10} ,item =>
                    {
                        var task = GetDataAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{item["id"]}"), accessToken);
                        taskList.Add(task);
                    });

                    skip += size;

                    remainingRecords = totalNoOfRecords - skip;

                    totalRecords.AddRange(jsonArray.Children());
                }
                catch (Exception ex)
                {

                }
            } while (remainingRecords > 0);
            

            var taskResults = await Task.WhenAll(taskList);

            for (int i = 0; i < taskList.Count; i++)
            {
                var result = taskResults[i];
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
            var authResponse = await _meaClient.ReturnAuthorizationTokenAsync().ConfigureAwait(false);

            var accessToken = await authResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            return await GetDataAsync(url, accessToken).ConfigureAwait(false);
        }

        private Task<string> GetDataAsync(Uri url, string token)
        {
            return _meaClient.GetAsync(url, token);
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

