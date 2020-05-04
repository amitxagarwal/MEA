using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.TaskApi.Model
{
    public class TaskSearchQuery
    {
        public IReadOnlyList<string> AssignedActors { get; set; }
        public IReadOnlyList<TaskType> Types { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsStarted { get; set; }
        public bool IncludeUnassignedTasks { get; set; }
    }
}
