using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Common
{
    public interface IHelperHttpClient
    {
        Task<HttpResponseMessage> GetDataFromMomentumCore(IConfiguration config, Uri url, string httpMethod = "get", StringContent requestBody = null);
    }
}
