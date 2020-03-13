using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class CitizenDataModel
    {
        public string Cpr { get; set; }

        public int Age
        {
            get
            {
                var age = DateTime.Now.Year - BirthDate.Year;
                var now = DateTime.Now;
                if (now.Month < BirthDate.Month || (now.Month == BirthDate.Month && now.Day < BirthDate.Day))
                {
                    age--;
                }

                return age;
            }
        }

        public int ResidenceMunicipalityId { get; set; }
        public int? ResponsibleMunicipalityId { get; set; }
        public int JobcenterId { get; set; }
        public bool HasProtectedAddress { get; set; }
        public IEnumerable<ResponsibleActors> ResponsibleActors { get; set; }
        public DateTime BirthDate { get; set; }
        public ContactInformation ContactInformation { get; set; }
        public bool IsExternalLookup { get; set; }
        public ClassificationStatus? ClassificationStatus { get; private set; }
        public string TargetGroupCode { get; private set; }
        public bool IsBelongingToOtherJobCenter { get; set; }
        public string JobCenterName { get; set; }
        public CitizenProfile Profile { get; set; }
        public bool IsDead { get; private set; }
        public string MaritalStatusCode { get; set; }
        public string CitizenshipCode { get; set; }
        public CitizenEnrollmentStatus EnrollmentStatus { get; set; }
        public string CurrentContactGroupCode { get; private set; }
        public string CurrentPersonCategory { get; private set; }
        public bool? IsHandledInUnemploymentFund { get; set; }
        public string PersonCivilRegistrationStatus { get; set; }

        public Classification CurrentClassification
        {
            set
            {
                if (value == null)
                {
                    return;
                }

                CurrentPersonCategory = (value as IHasPersonCategoryCode)?.PersonCategoryCode;
                CurrentContactGroupCode = value.ContactGroupCode;
            }
        }

        public Classification LatestClassification
        {
            set
            {
                if (value == null)
                {
                    return;
                }


                IsDead = value.ClosingReasonCode == ClassificationClosingReasons.Doed ||
                         (!string.IsNullOrEmpty(PersonCivilRegistrationStatus) && (PersonCivilRegistrationStatus.Equals(CprStatusTypes.Dead)));
                ClassificationStatus = value.Status;

                if (!value.IsActive)
                {
                    return;
                }

                TargetGroupCode = value.TargetGroupCode;
            }
        }
    }
}
