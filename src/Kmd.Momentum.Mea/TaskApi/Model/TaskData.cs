using System;
using System.Collections.Generic;

namespace Kmd.Momentum.Mea.TaskApi.Model
{
    public class TaskData
    {
        public string SicknessBenefitType { get; set; }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskState State { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedById { get; set; }
        public string CreatedById { get; set; }
        public DateTime? StateChangedAt { get; set; }
        public IReadOnlyList<string> Assignee { get; set; }
        public IReadOnlyList<AssignedActors> AssignedActors { get; set; }
        public string Subject { get; set; }
        public int Type { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public Reference Reference { get; set; }
        public IReadOnlyList<Participants> Participants { get; set; }
        public int? RelatedEntityType { get; set; }
    }
}
