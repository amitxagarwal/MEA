using System.Collections.Generic;

namespace Kmd.Momentum.Mea.TaskApi.Model
{
    public class TaskList
    {
        public int TotalNoOfPages { get; set; }

        public int TotalSearchCount { get; set; }

        public int PageNo { get; set; }

        public IReadOnlyList<TaskDataResponseModel> Result { get; set; }
    }
}
