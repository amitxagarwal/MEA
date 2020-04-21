using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Caseworker.Model
{
    public class McaPUnitData
    {
        public string ReferenceId { get; set; }

        public int TotalPages { get; set; }

        public int TotalSearchCount { get; set; }

        public int Page { get; set; }

        public bool HasMore { get; set; }

        public string AdditionalValues { get; set; }

        public IReadOnlyList<McaCaseworkerDataResponse> Data { get; set; }
    }
}

