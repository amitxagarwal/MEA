using System;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Test.Common
{
    public interface ITokenHelper
    {
        Task<String> GetToken();
    }
}
