using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Caseworker
{
    public interface ICitizenHttpClient
    {
        Task ReturnAuthorizationToken();
    }
}
