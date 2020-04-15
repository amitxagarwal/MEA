namespace Kmd.Momentum.Mea.Caseworker.Model
{
    public class CaseworkerDataResponse
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public string GivenName { get; set; }

        public string MiddleName { get; set; }

        public string Initials { get; set; }

        public Email Email { get; set; }

        public Phone Phone { get; set; }

        public string CaseworkerIdentifier { get; set; }

        public string Description { get; set; }

        public bool IsBookable { get; set; }

        public bool IsActive { get; }

        public CaseworkerDataResponse(string id, string displayName, string givenName, string middleName, string initials,
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


