using Newtonsoft.Json;
using System;

namespace Kmd.Momentum.Mea.Citizen.Model
{
    public class CitizenDataResponseModel
    {
        [JsonProperty("id")]
        public Guid CitizenId { get; }

        [JsonProperty("name")]
        public string DisplayName { get; }

        [JsonProperty("givenName")]
        public string GivenName { get; }

        [JsonProperty("middleName")]
        public string MiddleName { get; }

        [JsonProperty("initials")]
        public string Initials { get; }

        [JsonProperty("address")]
        public string Email { get; }

        [JsonProperty("number")]
        public string Phone { get; }

        [JsonProperty("caseworkerIdentifier")]
        public string CaseworkerIdentifier { get; }

        [JsonProperty("description")]
        public string Description { get; }

        [JsonProperty("isBookable")]
        public bool IsBookable { get; }

        [JsonProperty("isActive")]
        public bool IsActive { get; }

        public CitizenDataResponseModel(Guid citizenId, string displayName, string givenName, string middleName, string initials, string email, string phone,
            string caseworkerIdentifier, string description,
            bool isActive = true, bool isBookable = true)
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
            IsBookable = isBookable;
            IsActive = isActive;
        }
    }
}