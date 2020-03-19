using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Citizen
{
    public interface ICitizenService
    {
        Task<IReadOnlyList<CitizenListResponse>> GetAllActiveCitizensAsync();

        Task<CitizenDataResponse> GetCitizenByCprAsync(string cpr);

        Task<CitizenDataResponse> GetCitizenByIdAsync(string citizenId);
    }
}
