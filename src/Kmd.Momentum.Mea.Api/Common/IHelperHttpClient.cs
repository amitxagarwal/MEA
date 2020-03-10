using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Common
{
    public interface IHelperHttpClient
    {
        Task<HttpResponseMessage> ReturnAuthorizationToken(IConfiguration config);

        Task<HttpResponseMessage> CallMCA(IConfiguration config, Uri url, string httpMethod = "get", StringContent requestBody = null);

    }
}
