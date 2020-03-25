using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.MeaHttpClient
{
    public interface IMeaClient
    {
        Task<string> GetAsync(Uri uri, string token);
        Task<string> PostAsync(Uri uri, StringContent stringContent, string token);
        Task<HttpResponseMessage> ReturnAuthorizationTokenAsync();
    }
}
