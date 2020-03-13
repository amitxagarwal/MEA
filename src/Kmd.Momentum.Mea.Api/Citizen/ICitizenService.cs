using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    public interface ICitizenService
    {
        Task<string[]> GetAllActiveCitizens();

        Task<CitizenDataResponse> GetCitizenByCpr(string cpr);

        Task<CitizenDataResponse> GetCitizenById(string citizenId);
    }
}
