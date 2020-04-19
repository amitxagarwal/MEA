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

        public async Task<ResultOrHttpError<IReadOnlyList<MeaBaseList>, Error>> GetAllCaseworkerDataFromMomentumCoreAsync(Uri url, int pageNumber)
        {
            
            var pageSize = 50;
           

            List<CaseworkerDataResponseModel> totalRecords = new List<CaseworkerDataResponseModel>();

            
            
                var PageNumber = pageNumber;
                var queryStringParams = $"pagingInfo.pageNumber={PageNumber}&pagingInfo.pageSize={pageSize}";
                var response = await _meaClient.GetAsync(new Uri(url + "?" + queryStringParams)).ConfigureAwait(false);

                if (response.IsError)
                {
                    return new ResultOrHttpError<IReadOnlyList<MeaBaseList>, Error>(response.Error, response.StatusCode.Value);
                }

                var content = response.Result;
                var citizenDataObj = JsonConvert.DeserializeObject<PUnitData>(content);
                var records = citizenDataObj.Data;
            var y = new MeaBaseList()
            {
                PageNo = PageNumber,
                TotalNoOfPages = citizenDataObj.TotalPages,
                TotalSearchCount = citizenDataObj.TotalSearchCount,
                Result = totalRecords.ToArray()
            };

            foreach (var item in records)
                {
                    var x = new CaseworkerDataResponseModel(item.Id, item.DisplayName, item.GivenName, item.MiddleName, item.Initials,
                   item.Email?.Address, item.Phone?.Number, item.CaseworkerIdentifier, item.Description, item.IsActive, item.IsBookable);
                    totalRecords.Add(x);
                }

           


            return new ResultOrHttpError<IReadOnlyList<MeaBaseList>, Error>(y);
        }

        public async Task<ResultOrHttpError<string, Error>> GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync(Uri url)
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