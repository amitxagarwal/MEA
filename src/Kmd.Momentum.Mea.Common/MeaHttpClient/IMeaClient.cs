using Kmd.Momentum.Mea.Common.Exceptions;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.MeaHttpClient
{
    public interface IMeaClient
    {
        Task<ResultOrHttpError<string, Error>> GetAsync(string path);
        Task<ResultOrHttpError<string, Error>> PostAsync(string path, StringContent stringContent);
        Task<ResultOrHttpError<string, Error>> PutAsync(string path);
    }
}
