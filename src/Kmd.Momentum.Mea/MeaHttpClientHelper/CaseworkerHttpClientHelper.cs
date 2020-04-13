using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Caseworker1.Model;
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

        public async Task<IReadOnlyList<ClaseworkerData>> GetAllCaseworkerDataFromMomentumCoreAsync(Uri url)
        {
            var PageNumber = 0;
            var pageSize = 50;
            bool hasMore = true;
            List<ClaseworkerData> totalRecords = new List<ClaseworkerData>();
            while (hasMore)
            {
                PageNumber++;
                var queryStringParams = $"pagingInfo.pageNumber={PageNumber}&pagingInfo.pageSize={pageSize}";
                var response = await _meaClient.GetAsync(new Uri(url + "?" + queryStringParams)).ConfigureAwait(false);
                var content = response.Result;
                var citizenDataObj = JsonConvert.DeserializeObject<PUnitData>(content);
                var records = citizenDataObj.Data;

                //    ClaseworkerData data = new ClaseworkerData() { CaseworkerId,DisplayName,GivenName, MiddleName, Initials, Email,  Phone,
                //CaseworkerIdentifier,  Description,
                //IsActive = true,  IsBookable = true};

                foreach (var item in records)
                {
                    if (item.Phone == null && item.Email == null)
                    {
                        var x = new ClaseworkerData(item.CaseworkerId, item.DisplayName, item.GivenName, item.MiddleName, item.Initials, null,
                       null, item.CaseworkerIdentifier, item.Description, item.IsActive, item.IsBookable);
                        totalRecords.Add(x);
                    }

                    else if(item.Phone != null && item.Email != null)
                    {
                        var x = new ClaseworkerData(item.CaseworkerId, item.DisplayName, item.GivenName, item.MiddleName, item.Initials, item.Email.Address,
                      item.Phone.Number, item.CaseworkerIdentifier, item.Description, item.IsActive, item.IsBookable);
                        totalRecords.Add(x);
                    }

                    else if (item.Phone != null && item.Email == null)
                    {
                        var x = new ClaseworkerData(item.CaseworkerId, item.DisplayName, item.GivenName, item.MiddleName, item.Initials, null,
                      item.Phone.Number, item.CaseworkerIdentifier, item.Description, item.IsActive, item.IsBookable);
                        totalRecords.Add(x);
                    }

                    else
                    {
                        var x = new ClaseworkerData(item.CaseworkerId, item.DisplayName, item.GivenName, item.MiddleName, item.Initials, item.Email.Address,
                      null, item.CaseworkerIdentifier, item.Description, item.IsActive, item.IsBookable);
                        totalRecords.Add(x);
                    }

                }

                hasMore = citizenDataObj.HasMore;
            }
            return totalRecords;
        }
    }
}




