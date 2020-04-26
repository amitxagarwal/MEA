using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Citizen.Model
{
    public class CitizenList
    {
        public int TotalNoOfPages { get; }

        public int TotalSearchCount { get; }

        public int PageNo { get; }

        public IReadOnlyList<CitizenDataResponseModel> Result { get; }

        public CitizenList(int totalNoOfPages, int totalSearchCount, int pageNo, IReadOnlyList<CitizenDataResponseModel> result)
        {
            TotalNoOfPages = totalNoOfPages;
            TotalSearchCount = totalSearchCount;
            PageNo = pageNo;
            Result = result;
        }
    }
}