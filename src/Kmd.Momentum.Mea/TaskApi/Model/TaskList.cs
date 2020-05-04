using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Task.Model
{
    public class TaskList
    {
        public int TotalNoOfPages { get; set; }

        public int TotalSearchCount { get; set; }

        public int PageNo { get; set; }

        public IReadOnlyList<TaskDataResponseModel> Result { get; set; }
    }
}
