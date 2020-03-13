using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    public interface ICitizenService
    {
        Task<string[]> GetDataFromMomentumCore();

        Task<CitizenDataResponse> GetCitizenByCpr(string cpr);

        Task<CitizenDataResponse> GetCitizenById(string citizenId);
    }
}
