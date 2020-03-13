using System;
using System.Collections.Generic;

namespace Kmd.Momentum.Mea.Api
{
    public class Classification
    {
        public DateTime Start { get; protected set; }

        public DateTime? End { get; set; }

        public DateTime ActiveFrom { get; protected set; }

        public Guid CitizenId { get; set; }

        public ICollection<ClassificationSnapshot> Snapshots { get; set; }
        public ICollection<ComponentIdentifier> ComponentIdentifiers { get; set; }
        public ClassificationStatus Status { get; set; }
        public string ContactGroupCode { get; set; }

        public string ClosingReasonCode { get; set; }
        public Taxon ClosingReason { get; set; }
        public string Comment { get; set; }

        public virtual string TargetGroupCode { get; protected set; }

        public Taxon TargetGroup { get; protected set; }
        public Taxon CaseTag { get; protected set; }
        public string CaseTagCode { get; set; }
        public Taxon CaseActionFacetTag { get; protected set; }
        public string CaseActionFacetTagCode { get; set; }
        public Case Case { get; protected set; }

        // TODO: KLE US81625 should no longer be nullable
        public Guid? CaseId { get; protected set; }

        /// <summary>
        /// Name of this one is misleading - the real purpose of this flag is to express if user 
        /// can decide not to create enrollment if there are any client categories which meets the criteria as stated in GetPossibleEnrollmentTypes
        /// </summary>
        public abstract bool RequiresEnrollment { get; }

        public abstract string[] EditableFields { get; }
        public abstract IEnumerable<PersonCategory> PersonCategories { get; }
        public abstract IEnumerable<ContactGroup> ContactGroups { get; }
        public IEnumerable<ClientCategory> ClientCategories => ClientCategoryCodes.GetRelevantForTargetGroup(GetType());
        public bool SkipVersioning { get; set; }
        public IList<EventOperationType> Operations { get; } = new List<EventOperationType>();

        public virtual bool IsApplicableForTJob => false;
    }
}