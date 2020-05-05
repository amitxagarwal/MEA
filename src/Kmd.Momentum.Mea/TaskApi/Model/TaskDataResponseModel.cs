using System.Collections.Generic;

namespace Kmd.Momentum.Mea.TaskApi.Model
{
    public class TaskDataResponseModel
    {
        public string TaskId { get; }

        public string Title { get; }

        public string Description { get; }

        public string Deadline { get; }

        public string CreateAt { get; }

        public string StateChangedAt { get; }
        public string TaskState { get; }

        public IReadOnlyList<AssignedActors> AssignedActors { get; }
        public Reference Reference { get; }


        public TaskDataResponseModel(string taskId, string title, string description, string deadline, string createAt,
            string stateChangedAt, string taskState, IReadOnlyList<AssignedActors> assignedActors, Reference reference)
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
