using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    public interface ICitizenService
    {
        Task<string[]> GetAllActiveCitizens();

        Task<CitizenDataResponse> getCitizenByCpr(string cpr);

        Task<CitizenDataResponse> getCitizenById(string citizenId);
    }
}
