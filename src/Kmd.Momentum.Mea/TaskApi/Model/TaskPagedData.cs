using System.Collections;
using System.Collections.Generic;

namespace Kmd.Momentum.Mea.TaskApi.Model
{
   public class TaskPagedData
    {
        public string ReferenceId { get; set; }

        public int TotalPages { get; set; }

        public int TotalSearchCount { get; set; }

        public int Page { get; set; }

        public bool HasMore { get; set; }

        public IDictionary AdditionalValues { get; set; }

        public IReadOnlyList<TaskData> Data { get; set; }
    }
}
