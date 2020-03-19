using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Citizen
{
    public interface ICitizenService
    {
        Task<string[]> GetAllActiveCitizensAsync();

        Task<ResultOrHttpError<CitizenDataResponseModel, string>> GetCitizenByCprAsync(string cpr);

        Task<ResultOrHttpError<CitizenDataResponseModel, string>> GetCitizenByIdAsync(string citizenId);
    }
}
