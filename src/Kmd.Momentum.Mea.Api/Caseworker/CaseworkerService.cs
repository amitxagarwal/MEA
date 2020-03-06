using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;


namespace Kmd.Momentum.Mea.Api
{
    public class CaseworkerService : ICaseworkerService
    {
#pragma warning disable CA1822 // Mark members as static
            public async Task<CaseworkerResponse> GetCaseworkerDetailsAsync(Guid Id, CaseworkerRequest request)
#pragma warning restore CA1822 // Mark members as static
        {
            await Task.Delay(300).ConfigureAwait(false);
            var caseWorkerData = new CaseworkerData(
                Id,
                "",
                "",
                request?.query
                 ) ;

            var response = new CaseworkerResponse() { citizenName = caseWorkerData.Name, document = caseWorkerData.Municipality };
            return response;
        }
    }
}
