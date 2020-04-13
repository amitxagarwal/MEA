using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public class CaseworkerHttpClientHelper : ICaseworkerHttpClientHelper
    {
        private readonly IMeaClient _meaClient;

        public CaseworkerHttpClientHelper(IMeaClient meaClient)
        {
            _meaClient = meaClient ?? throw new ArgumentNullException(nameof(meaClient));
        }

        public async Task<ResultOrHttpError<IReadOnlyList<CaseworkerDataResponse>, Error>> GetAllCaseworkerDataFromMomentumCoreAsync(Uri url)
        {
            var PageNumber = 0;
            var pageSize = 50;
            bool hasMore = true;

            List<CaseworkerDataResponse> totalRecords = new List<CaseworkerDataResponse>();

            while (hasMore)
            {
                PageNumber++;
                var queryStringParams = $"pagingInfo.pageNumber={PageNumber}&pagingInfo.pageSize={pageSize}";
                var response = await _meaClient.GetAsync(new Uri(url + "?" + queryStringParams)).ConfigureAwait(false);

                if (response.IsError)
                {
                    return new ResultOrHttpError<IReadOnlyList<CaseworkerDataResponse>, Error>(response.Error, response.StatusCode.Value);
                }

                var content = response.Result;
                var citizenDataObj = JsonConvert.DeserializeObject<PUnitData>(content);
                var records = citizenDataObj.Data;

                foreach (var item in records)
                {
                    var x = new CaseworkerDataResponse(item.CaseworkerId, item.DisplayName, item.GivenName, item.MiddleName, item.Initials,
                    item.CaseworkerIdentifier, item.Description, item.IsActive, item.IsBookable, item.Email?.Address, item.Phone?.Number);
                    totalRecords.Add(x);
                }

                hasMore = citizenDataObj.HasMore;
            }
            return new ResultOrHttpError<IReadOnlyList<CaseworkerDataResponse>, Error>(totalRecords);
        }
    }
}




