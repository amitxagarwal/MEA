using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.MeaHttpClient
{
    public interface IMeaClient
    {
        Task<string> GetAsync(Uri uri);
        Task<string> PostAsync(Uri uri, StringContent stringContent);
    }
}
