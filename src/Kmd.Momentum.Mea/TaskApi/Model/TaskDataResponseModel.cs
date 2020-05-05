using System;
using System.Collections.Generic;

namespace Kmd.Momentum.Mea.TaskApi.Model
{
    public class TaskDataResponseModel
    {
        public Guid TaskId { get; }

        public string Title { get; }

        public string Description { get; }

        public DateTime Deadline { get; }

        public DateTime CreateAt { get; }

        public DateTime? StateChangedAt { get; }
        public TaskState TaskState { get; }

        public IReadOnlyList<AssignedActors> AssignedActors { get; }
        public Reference Reference { get; }


        public TaskDataResponseModel(Guid taskId, string title, string description, DateTime deadline, DateTime createAt,
            DateTime? stateChangedAt, TaskState taskState, IReadOnlyList<AssignedActors> assignedActors, Reference reference)
        {
            TaskId = taskId;
            Title = title;
            Description = description;
            Deadline = deadline;
            CreateAt = createAt;
            StateChangedAt = stateChangedAt;
            TaskState = taskState;
            AssignedActors = assignedActors;
            Reference = reference;
        }
    }
}
