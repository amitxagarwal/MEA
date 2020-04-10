using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Caseworker1.Model
{
    public class PUnitData
    {
        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }

        [JsonProperty("totalSearchCount")]
        public int TotalSearchCount { get; set; }

        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

        [JsonProperty("data")]
        public IReadOnlyList<CaseworkerDataResponseModel> Data { get; set; }
    }

    }

    public class CaseworkerDataResponseModel
    {
        [JsonProperty("id")]
        public string CaseworkerId { get; }

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

        public CaseworkerDataResponseModel(string caseworkerId, string displayName, string givenName, string middleName, string initials, string email, string phone,
           string caseworkerIdentifier, string description,
           bool isActive = true, bool isBookable = true)
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


