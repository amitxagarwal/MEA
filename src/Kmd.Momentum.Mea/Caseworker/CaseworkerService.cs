using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Caseworker1.Model;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Caseworker1
{
    public class CaseworkerService : ICaseworkerService
    {
        private readonly ICaseworkerHttpClientHelper _caseworkerHttpClient;
        private readonly IConfiguration _config;

        public CaseworkerService(ICaseworkerHttpClientHelper caseworkerHttpClient, IConfiguration config)
        {
            _caseworkerHttpClient = caseworkerHttpClient;
            _config = config;
        }

        public async Task<IReadOnlyList<ClaseworkerData>> GetAllCaseworkersInMomentumAsync()
        {
            var response = await _caseworkerHttpClient.GetAllCaseworkerDataFromMomentumCoreAsync(new Uri($"{_config["KMD_MOMENTUM_MEA_McaApiUri"]}/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers")).ConfigureAwait(false);

            return response;
        }


    }
}
    

