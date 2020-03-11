using Kmd.Momentum.Mea.Api.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    public class CitizenService : ICitizenService
    {
        private readonly IHelperHttpClient _citizenHttpClient;

        private readonly IConfiguration _config;

        public CitizenService(IHelperHttpClient citizenHttpClient, IConfiguration config)
        {
            _citizenHttpClient = citizenHttpClient;
            _config = config;
        }
        public async Task<string[]> GetAllActiveCitizens()
        {
            HttpResponseMessage response = await _citizenHttpClient.CallMCA(_config, new Uri($"{_config["McaUri"]}citizens/withActiveClassification"), "get").ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<string[]>(json);
            }
            throw new Exception(response.StatusCode.ToString());
        }

        public async Task<string[]> GetCitizenById(Guid citizenId)
        {
            HttpResponseMessage response = await _citizenHttpClient.CallMCA(_config, new Uri($"{_config["McaUri"]}citizens/0809982483"), "get").ConfigureAwait(false);

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<string[]>(json);

        }
    }
}
