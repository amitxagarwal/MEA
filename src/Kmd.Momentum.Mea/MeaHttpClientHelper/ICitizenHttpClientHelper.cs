using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface ICitizenHttpClientHelper
    {
        Task<ResultOrHttpError<CitizenList, Error>> GetAllActiveCitizenDataFromMomentumCoreAsync(string path, int pageNumber);

        Task<ResultOrHttpError<string, Error>> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(string path);

        Task<ResultOrHttpError<string, Error>> CreateJournalNoteInMomentumCoreAsync(string path, string momentumCitizenId, JournalNoteRequestModel requestModel);
    }
}
