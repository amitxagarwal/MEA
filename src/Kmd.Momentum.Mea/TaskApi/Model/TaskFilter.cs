using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Task.Model
{
   public class TaskFilter
    {
        public string ReferenceId { get; set; }

        public int TotalPages { get; set; }

        public int TotalSearchCount { get; set; }

        public int Page { get; set; }

        public bool HasMore { get; set; }

        public string AdditionalValues { get; set; }

        public IReadOnlyList<TaskData> Data { get; set; }
    }
}
