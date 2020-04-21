namespace Kmd.Momentum.Mea.Caseworker.Model
{
    public class McaCaseworkerDataResponse
    {
        public string Id { get; }

        public string DisplayName { get; }

        public string GivenName { get; }

        public string MiddleName { get; }

        public string Initials { get; }

        public Email Email { get; }

        public Phone Phone { get; }

        public string CaseworkerIdentifier { get; }

        public string Description { get; }

        public bool IsBookable { get; }

        public bool IsActive { get; }

        public McaCaseworkerDataResponse(string id, string displayName, string givenName, string middleName, string initials,
           string caseworkerIdentifier, string description,
           bool isActive = true, bool isBookable = true, Email email = null, Phone phone = null)
        {
            Id = id;
            DisplayName = displayName;
            GivenName = givenName;
            MiddleName = middleName;
            Initials = initials;
            Email = email;
            Phone = phone;
            CaseworkerIdentifier = caseworkerIdentifier;
            Description = description;
            IsBookable = isBookable;
            IsActive = isActive;
        }
    }
}


