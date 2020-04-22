using Kmd.Momentum.Mea.Common.Exceptions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.MeaHttpClient
{
    public interface IMeaClient
    {
        Task<ResultOrHttpError<string, Error>> GetAsync(Uri uri);

        Task<ResultOrHttpError<string, Error>> PostAsync(Uri uri, StringContent stringContent);
    }
}
