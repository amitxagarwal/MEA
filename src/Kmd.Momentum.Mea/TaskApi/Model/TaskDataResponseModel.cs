using System;
using System.Collections.Generic;
using System.Text;

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


        public TaskDataResponseModel(string taskId, string title, string description, string deadline, string createAt,
            string stateChangedAt)
        {
            TaskId = taskId;
            Title = title;
            Description = description;
            Deadline = deadline;
            CreateAt = createAt;
            StateChangedAt = stateChangedAt;
           
        }
    }
}
