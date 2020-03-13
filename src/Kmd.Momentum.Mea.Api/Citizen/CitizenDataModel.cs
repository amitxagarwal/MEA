using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class CitizenDataModel
    {
        public string Cpr { get; }
        public int Age { get; }
        public int? ResidenceMunicipalityId { get; }
        public int? ResponsibleMunicipalityId { get; }
        public int JobcenterId { get; }
        public bool HasProtectedAddress { get; }
        public ResponsibleActors[] ResponsibleActors { get; }
        public DateTimeOffset BirthDate { get; }
        public ContactInformation ContactInformation { get; }
        public bool IsExternalLookup { get; }
        public int? ClassificationStatus { get; }
        public string TargetGroupCode { get; }
        public bool IsBelongingToOtherJobCenter { get; }
        public string JobCenterName { get; }
        public Profile Profile { get; }
        public string IsDead { get; }
        public string MaritalStatusCode { get; }
        public string CitizenshipCode { get; }
        public EnrollmentStatus EnrollmentStatus { get; }
        public string CurrentContactGroupCode { get; }
        public string CurrentPersonCategory { get; }
        public string IsHandledInUnemploymentFund { get; }
        public string PersonCivilRegistrationStatus { get; }
        public string Id { get; }
        public string DisplayName { get; }

        public CitizenDataModel(string cpr, int age, int residenceMunicipalityId, int responsibleMunicipalityId, int jobcenterId, bool hasProtectedAddress,
            ResponsibleActors[] responsibleActors, DateTimeOffset birthDate, ContactInformation contactInformation, bool isExternalLookup, int classificationStatus,
            string targetGroupCode, bool isBelongingToOtherJobCenter, string jobCenterName, Profile profile, string isDead, string maritalStatusCode,
            string citizenshipCode, EnrollmentStatus enrollmentStatus, string currentContactGroupCode, string currentPersonCategory, string isHandledInUnemploymentFund,
            string personCivilRegistrationStatus, string id, string displayName)
        {
            Cpr = cpr;
            Age = age;
            ResidenceMunicipalityId = residenceMunicipalityId;
            ResponsibleMunicipalityId = responsibleMunicipalityId;
            JobcenterId = jobcenterId;
            HasProtectedAddress = hasProtectedAddress;
            ResponsibleActors = responsibleActors;
            BirthDate = birthDate;
            ContactInformation = contactInformation;
            IsExternalLookup = isExternalLookup;
            ClassificationStatus = classificationStatus;
            TargetGroupCode = targetGroupCode;
            IsBelongingToOtherJobCenter = isBelongingToOtherJobCenter;
            JobCenterName = jobCenterName;
            Profile = profile;
            IsDead = isDead;
            MaritalStatusCode = maritalStatusCode;
            CitizenshipCode = citizenshipCode;
            EnrollmentStatus = enrollmentStatus;
            CurrentContactGroupCode = currentContactGroupCode;
            CurrentPersonCategory = currentPersonCategory;
            IsHandledInUnemploymentFund = isHandledInUnemploymentFund;
            PersonCivilRegistrationStatus = personCivilRegistrationStatus;
            Id = id;
            DisplayName = displayName;
        }
    }
}
