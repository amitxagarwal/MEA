using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Citizen
{
    public interface ICitizenService
    {
        Task<ResultOrHttpError<CitizenList, Error>> GetAllActiveCitizensAsync(int pageNumber);

        Task<ResultOrHttpError<CitizenDataResponseModel, Error>> GetCitizenByCprAsync(string cpr);

        Task<ResultOrHttpError<CitizenDataResponseModel, Error>> GetCitizenByIdAsync(string citizenId);

        Task<ResultOrHttpError<string, Error>> CreateJournalNoteAsync(string momentumCitizenId, JournalNoteResponseModel requestModel);
    }
}
