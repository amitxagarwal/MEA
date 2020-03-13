namespace Kmd.Momentum.Mea.Api
{
    public class CitizenDataResponse
    {
        public string Id { get; }
        public bool IsBookable { get; }
        public string DisplayName { get; }
        public string GivenName { get; }
        public string MiddleName { get; }
        public string Initials { get; }
        public string Email { get; }
        public string Phone { get; }
        public string CaseworkerIdentifier { get; }
        public string Description { get; }
        public bool IsActive { get; }

        public CitizenDataResponse(string id, bool isBookable, string displayName, string givenName, string middleName, string initials, string email, string phone, string caseworkerIdentifier, string description, bool isActive)
        {
            Id = id;
            IsBookable = isBookable;
            DisplayName = displayName;
            GivenName = givenName;
            MiddleName = middleName;
            Initials = initials;
            Email = email;
            Phone = phone;
            CaseworkerIdentifier = caseworkerIdentifier;
            Description = description;
            IsActive = isActive;
        }
    }
}
