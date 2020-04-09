using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
     public class CaseworkerHttpClientHelper : ICaseworkerHttpClientHelper
    {
        private readonly IMeaClient _meaClient;

        public CaseworkerHttpClientHelper(IMeaClient meaClient)
        {
            _meaClient = meaClient;
        }



        public async Task<ResultOrHttpError<IReadOnlyList<string>, Error>> GetAllCaseworkerDataFromMomentumCoreAsync(Uri url)
        {
            List<JToken> totalRecords = new List<JToken>();
            List<string> JsonStringList = new List<string>();

            var size = 100;
            var skip = 0;

            int remainingRecords;

            do
            {
                var queryStringParams = $"term=Caseworker&size={size}&skip={skip}&isActive=true";
                var response = await _meaClient.GetAsync(new Uri(url + "?" + queryStringParams)).ConfigureAwait(false);

                if (response.IsError)
                {
                    return new ResultOrHttpError<IReadOnlyList<string>, Error>(response.Error, response.StatusCode.Value);
                }

                var content = response.Result;
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
                    caseworkerId = item["id"],
                    displayName = item["name"],
                    givenName = (string)null,
                    middleName = (string)null,
                    initials = (string)null,
                    address = (string)null,
                    number = (string)null,
                    caseworkerIdentifier = item["identifier"],
                    description = item["description"],
                    isBookable = true,
                    isActive = item["isActive"]
                });
                JsonStringList.Add(jsonToReturn);

            }
            return new ResultOrHttpError<IReadOnlyList<string>, Error>(JsonStringList);
        }
        public async Task<ResultOrHttpError<string, Error>> GetDataByMomentumIdFromMomentumCoreAsync(Uri url)
        {
            var response = await _meaClient.GetAsync(url).ConfigureAwait(false);

            if (response.IsError)
            {
                return new ResultOrHttpError<string, Error>(response.Error, response.StatusCode.Value);
            }

            var content = response.Result;
            return new ResultOrHttpError<string, Error>(content);
        }
    }
}
