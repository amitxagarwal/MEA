using Kmd.Momentum.Mea.Citizen.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Citizen
{
    public interface ICitizenService
    {
        Task<IReadOnlyList<CitizenDataResponseModel>> GetAllActiveCitizensAsync();

        Task<CitizenDataResponseModel> GetCitizenByCprAsync(string cpr);

        Task<CitizenDataResponseModel> GetCitizenByIdAsync(string citizenId);
    }
}
