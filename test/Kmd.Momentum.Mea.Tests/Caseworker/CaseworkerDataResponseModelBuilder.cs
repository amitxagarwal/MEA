using Kmd.Momentum.Mea.Caseworker.Model;
using Moq;
using System;

namespace Kmd.Momentum.Mea.Tests.Caseworker
{
    public class CaseworkerDataResponseModelBuilder
    {
        private Guid caseworkerId = It.IsAny<Guid>();
        private string displayName = It.IsAny<string>();
        private string givenName = It.IsAny<string>();
        private string middleName = It.IsAny<string>();
        private string initials = It.IsAny<string>();
        private string email = It.IsAny<string>();
        private string phone = It.IsAny<string>();
        private string caseworkerIdentifier = It.IsAny<string>();
        private string description = It.IsAny<string>();
        private bool IsBookable = It.IsAny<bool>();
        private bool IsActive = It.IsAny<bool>();

        public CaseworkerDataResponseModel Build()
        {
            return new CaseworkerDataResponseModel(caseworkerId, displayName, givenName, middleName, 
                initials, email, phone, caseworkerIdentifier, description, true, true);
        }

        public CaseworkerDataResponseModelBuilder WithCaseWorkerId(Guid caseworkerId)
        {
            this.caseworkerId = caseworkerId;
            return this;
        }

        public CaseworkerDataResponseModelBuilder WithDisplayName(string displayName)
        {
            this.displayName = displayName;
            return this;
        }
        public CaseworkerDataResponseModelBuilder WithGivenName(string givenName)
        {
            this.givenName = givenName;
            return this;
        }
        public CaseworkerDataResponseModelBuilder WithMiddleName(string middleName)
        {
            this.middleName = middleName;
            return this;
        }
        public CaseworkerDataResponseModelBuilder WithInitials(string initials)
        {
            this.initials = initials;
            return this;
        }
        public CaseworkerDataResponseModelBuilder WithEmail(string email)
        {
            this.email = email;
            return this;
        }
        public CaseworkerDataResponseModelBuilder WithPhone(string phone)
        {
            this.phone = phone;
            return this;
        }
        public CaseworkerDataResponseModelBuilder WithCaseworkerIdentifier(string caseworkerIdentifier)
        {
            this.caseworkerIdentifier = caseworkerIdentifier;
            return this;
        }
        public CaseworkerDataResponseModelBuilder With(string description)
        {
            this.description = description;
            return this;
        }
        public CaseworkerDataResponseModelBuilder WithIsBookable(bool IsBookable)
        {
            this.IsBookable = IsBookable;
            return this;
        }
        public CaseworkerDataResponseModelBuilder WithIsActive(bool IsActive)
        {
            this.IsActive = IsActive;
            return this;
        }
    }
}
