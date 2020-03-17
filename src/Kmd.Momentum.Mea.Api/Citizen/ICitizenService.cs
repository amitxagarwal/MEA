using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    public interface ICitizenService
    {
        Task<IReadOnlyList<CitizenSearchData>> GetAllActiveCitizensAsync();

        Task<CitizenDataResponse> GetCitizenByCprAsync(string cpr);

        Task<CitizenDataResponse> GetCitizenByIdAsync(string citizenId);
    }
}
