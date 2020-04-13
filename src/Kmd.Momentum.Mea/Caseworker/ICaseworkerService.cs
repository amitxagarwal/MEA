using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Caseworker1.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Caseworker1
{
   public interface ICaseworkerService
    {
        Task<IReadOnlyList<ClaseworkerData>> GetAllCaseworkersInMomentumAsync();
    }
}
