using Kmd.Momentum.Mea.Citizen.Model;

namespace Kmd.Momentum.Mea.Tests.Citizen
{
    public class CitizenDataResponseBuilder
    {
        private readonly string citizenId = "testCpr";
        private readonly string displayName = "testBody";
        private readonly string givenName = "testTitle";
        private readonly string middleName = "testType";
        private readonly string initials = "testType";
        private readonly string email = "testType";
        private readonly string phone = "testType";
        private readonly string caseworkerIdentifier = "testType";
        private readonly string description = "description";
        private readonly bool isBookable = true;
        private readonly bool isActive = true;

        public CitizenDataResponseModel Build()
        {
            return new CitizenDataResponseModel(citizenId, displayName, givenName, middleName, initials, email, phone, caseworkerIdentifier, description, true, true)
            {
                CitizenId = citizenId,
                DisplayName = displayName,
                GivenName = givenName,
                MiddleName = middleName,
                Initials = initials,
                Email = email,
                Phone = phone,
                CaseworkerIdentifier = caseworkerIdentifier,
                Description = description,
                IsBookable = isBookable,
                IsActive = isActive
            };
        }
    }
}
