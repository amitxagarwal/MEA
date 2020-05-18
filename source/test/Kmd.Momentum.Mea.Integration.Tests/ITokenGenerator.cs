using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Integration.Tests
{
    public interface ITokenGenerator
    {
        Task<string> GetToken();
    }
}
