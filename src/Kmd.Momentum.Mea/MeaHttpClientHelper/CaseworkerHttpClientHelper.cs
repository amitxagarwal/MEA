using Kmd.Momentum.Mea.Caseworker1;
using Kmd.Momentum.Mea.Caseworker1.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public async Task<IReadOnlyList<CaseworkerDataResponseModel>> GetAllCaseworkerDataFromMomentumCoreAsync(Uri url)
        {
          
         
            var PageNumber = -1;
            var pageSize = 50;
            bool hasMore = true;
            List<CaseworkerDataResponseModel> totalRecords = new List<CaseworkerDataResponseModel>();
            while (hasMore)
            {
                PageNumber++;
                var queryStringParams = $"pagingInfo.pageNumber={PageNumber}&pagingInfo.pageSize={pageSize}";
                var response = await _meaClient.GetAsync(new Uri(url + "?" + queryStringParams)).ConfigureAwait(false);
                var content = response.Result;
                var jsonArray = JArray.Parse(JObject.Parse(content).ToString());
                var citizenDataObj = JsonConvert.DeserializeObject<PUnitData>(jsonArray);
                var records = citizenDataObj.Data;
                totalRecords.AddRange(records);
                hasMore = citizenDataObj.HasMore;
            }
            return totalRecords;
         
        }
        
    }

        }


    

