using Kmd.Momentum.Mea.Citizen.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Citizen
{
    public interface ICitizenService
    {
        Task<IReadOnlyList<CitizenDataResponseModel>> GetAllActiveCitizensAsync();

        Task<ResultOrHttpError<CitizenDataResponseModel, string>> GetCitizenByCprAsync(string cpr);

        Task<ResultOrHttpError<CitizenDataResponseModel, string>> GetCitizenByIdAsync(string citizenId);
    }
}
