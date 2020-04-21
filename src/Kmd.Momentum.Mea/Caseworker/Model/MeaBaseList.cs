using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Caseworker.Model
{
    public class MeaBaseList
    {
        public int TotalNoOfPages { get; set; }

        public int TotalSearchCount { get; set; }

        public int PageNo { get; set; }

        public IReadOnlyList<MeaCaseworkerDataResponseModel> Result { get; set; }
    }
}
