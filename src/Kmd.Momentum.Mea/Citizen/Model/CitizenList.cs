using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Citizen.Model
{
    public class CitizenList
    {
        public int TotalNoOfPages { get; set; }

        public int TotalSearchCount { get; set; }

        public int PageNo { get; set; }

        public IReadOnlyList<CitizenDataResponseModel> Result { get; set; }
    }
}
