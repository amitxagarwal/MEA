namespace Kmd.Momentum.Mea.Api
{
    public class CitizenDataResponse
    {
        public string CitizenId { get; }
        public string DisplayName { get; }
        public string GivenName { get; }
        public string MiddleName { get; }
        public string Initials { get; }
        public string Email { get; }
        public string Phone { get; }
        public string CaseworkerIdentifier { get; }
        public string Description { get; }

        public CitizenDataResponse(string citizenId, string displayName, string givenName, string middleName, string initials, string email, string phone, string caseworkerIdentifier, string description)
        {
            CitizenId = citizenId;
            DisplayName = displayName;
            GivenName = givenName;
            MiddleName = middleName;
            Initials = initials;
            Email = email;
            Phone = phone;
            CaseworkerIdentifier = caseworkerIdentifier;
            Description = description;
        }
    }
}
