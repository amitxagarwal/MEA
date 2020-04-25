using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.MeaHttpClientHelper
{
    public interface ICitizenHttpClientHelper
    {
        Task<ResultOrHttpError<IReadOnlyList<string>, Error>> GetAllActiveCitizenDataFromMomentumCoreAsync(string path, int pageNumber);

        Task<ResultOrHttpError<string, Error>> GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(string path);

        Task<ResultOrHttpError<string, Error>> CreateJournalNoteInMomentumCoreAsync(string path, string momentumCitizenId, JournalNoteResponseModel requestModel);
    }
}
