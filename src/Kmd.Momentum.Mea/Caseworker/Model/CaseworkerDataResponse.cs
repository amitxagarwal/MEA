namespace Kmd.Momentum.Mea.Caseworker.Model
{
    public class CaseworkerDataResponse
    {
        public string CaseworkerId { get; }

        public string DisplayName { get; }

        public string GivenName { get; }

        public string MiddleName { get; }

        public string Initials { get; }

        public string Email { get; }

        public string Phone { get; }

        public string CaseworkerIdentifier { get; }

        public string Description { get; }

        public bool IsBookable { get; }

        public bool IsActive { get; }

        public CaseworkerDataResponse(string caseworkerId, string displayName, string givenName, string middleName, string initials,
           string caseworkerIdentifier, string description,
           bool isActive = true, bool isBookable = true, string email = null, string phone = null)
        {
            CaseworkerId = caseworkerId;
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


