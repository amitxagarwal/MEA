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

        public CitizenService(IHelperHttpClient citizenHttpClient)
        {
            _citizenHttpClient = citizenHttpClient;
        }
        public async Task<string[]> GetAllActiveCitizens(IConfiguration _config)
        {
            HttpResponseMessage response = await _citizenHttpClient.CallMCA(_config, new Uri($"https://kmd-rct-momentum-159-api.azurewebsites.net/api/citizens/withActiveClassification"), "get").ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<string[]>(json);
            }
            throw new Exception(response.StatusCode.ToString());
        }

        public async Task<string[]> GetCitizenById(IConfiguration _config, Guid citizenId)
        {
            // await _citizenHttpClient.ReturnAuthorizationToken().ConfigureAwait(false);

            HttpResponseMessage response = await _citizenHttpClient.CallMCA(_config, new Uri($"https://kmd-rct-momentum-159-api.azurewebsites.net/api/citizens/1610862558"), "get").ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<string[]>(json);
            }
            throw new Exception(response.StatusCode.ToString());
        }
    }
}
