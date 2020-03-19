using Kmd.Momentum.Mea.Citizen.Model;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Citizen
{
    public interface ICitizenService
    {
        Task<string[]> GetAllActiveCitizensAsync();

        Task<CitizenDataResponseModel> GetCitizenByCprAsync(string cpr);

        Task<CitizenDataResponseModel> GetCitizenByIdAsync(string citizenId);
    }
}
