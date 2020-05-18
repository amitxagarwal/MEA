using Kmd.Momentum.Mea.Citizen.Model;

namespace Kmd.Momentum.Mea.Tests.Citizen
{
    public class CitizenDataResponseModelBuilder
    {
        private string citizenId = "testCpr";
        private string displayName = "testBody";
        private string givenName = "testTitle";
        private string middleName = "testType";
        private string initials = "testType";
        private string email = "testType";
        private string phone = "testType";
        private string caseworkerIdentifier = "testType";
        private string description = "description";
        private bool isBookable = true;
        private bool isActive = true;

        public CitizenDataResponseModel Build()
        {
            return new CitizenDataResponseModel(citizenId, displayName, givenName, middleName, initials, email, phone, caseworkerIdentifier, description, true, true);
        }

        public CitizenDataResponseModelBuilder WithCpr(string citizenId)
        {
            this.citizenId = citizenId;
            return this;
        }
        public CitizenDataResponseModelBuilder WithDisplayName(string displayName)
        {
            this.displayName = displayName;
            return this;
        }
        public CitizenDataResponseModelBuilder WithGivenName(string givenName)
        {
            this.givenName = givenName;
            return this;
        }
        public CitizenDataResponseModelBuilder WithMiddleName(string middleName)
        {
            this.middleName = middleName;
            return this;
        }
        public CitizenDataResponseModelBuilder Withinitials(string initials)
        {
            this.initials = initials;
            return this;
        }
        public CitizenDataResponseModelBuilder WithEmail(string email)
        {
            this.email = email;
            return this;
        }
        public CitizenDataResponseModelBuilder WithPhone(string phone)
        {
            this.phone = phone;
            return this;
        }
        public CitizenDataResponseModelBuilder WithCaseworkerIdentifier(string caseworkerIdentifier)
        {
            this.caseworkerIdentifier = caseworkerIdentifier;
            return this;
        }
        public CitizenDataResponseModelBuilder WithDescription(string description)
        {
            this.description = description;
            return this;
        }
        public CitizenDataResponseModelBuilder WithIsBookable(bool isBookable)
        {
            this.isBookable = isBookable;
            return this;
        }
        public CitizenDataResponseModelBuilder WithIsActive(bool isActive)
        {
            this.isActive = isActive;
            return this;
        }
    }
}
