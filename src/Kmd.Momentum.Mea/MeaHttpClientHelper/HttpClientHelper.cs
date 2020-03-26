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

        public HttpClientHelper(IMeaClient meaClient)
        {
            _meaClient = meaClient;
        }

        public async Task<IReadOnlyList<string>> GetAllActiveCitizenDataFromMomentumCoreAsync(Uri url)
        {
            List<JToken> totalRecords = new List<JToken>();
            List<string> JsonStringList = new List<string>();

            var size = 100;
            var skip = 0;

            int remainingRecords;

            do
            {
                var queryStringParams = $"term=Citizen&size={size}&skip={skip}&isActive=true";
                var content = await _meaClient.GetAsync(new Uri(url + "?" + queryStringParams)).ConfigureAwait(false);

                var jsonArray = JArray.Parse(JObject.Parse(content)["results"].ToString());

                var totalNoOfRecords = (int)JProperty.Parse(content)["totalCount"];
                skip += size;

                remainingRecords = totalNoOfRecords - skip;
                totalRecords.AddRange(jsonArray.Children());

            } while (remainingRecords > 0);

            foreach (var item in totalRecords)
            {
                    var jsonToReturn = JsonConvert.SerializeObject(new
                    {
                        citizenId = item["id"],
                        displayName = item["name"],
                        givenName = "",
                        middleName = "",
                        initials = "",
                        address = "",
                        number = "",
                        caseworkerIdentifier = "",
                        description = item["description"],
                        isBookable = true,
                        isActive = true
                    });
                    JsonStringList.Add(jsonToReturn);
                
            }
            return JsonStringList;
        }

        public async Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(Uri url)
        {
            return await _meaClient.GetAsync(url).ConfigureAwait(false);
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

