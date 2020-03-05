using System;

namespace Kmd.Momentum.Mea.Api
{
    public class CaseworkerService : ICaseworkerService
    {
        Task<ResultOrHttpError<CaseworkerResponse, string>> CaseworkerAsync(CaseworkerRequest request)
        {

            var Request = new CaseworkerData(
                id,
               request.name,
               request.municipality
           );
            return Request;
        }
    }
}
