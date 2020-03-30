using Kmd.Momentum.Mea.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.MeaHttpClient
{
    public interface IMeaClient
    {
        Task<ResultOrHttpError<string, Error>> GetAsync(Uri uri);
        Task<string> PostAsync(Uri uri, StringContent stringContent);
    }
}
