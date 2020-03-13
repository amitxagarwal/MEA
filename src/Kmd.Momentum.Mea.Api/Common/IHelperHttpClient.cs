using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Common
{
    public interface IHelperHttpClient
    {
        Task<string[]> GetAllActiveCitizenDataFromMomentumCore(Uri url);
        Task<string> GetCitizenDataByCprOrCitizenIdFromMomentumCore(Uri url);        
    }
}
