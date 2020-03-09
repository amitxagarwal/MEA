using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Common
{
    public interface ICitizenHttpClient
    {
        Task<string> ReturnAuthorizationToken();
    }
}
