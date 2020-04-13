﻿using Kmd.Momentum.Mea.Caseworker;
using Newtonsoft.Json;

namespace Kmd.Momentum.Mea.Caseworker1.Model
{
    public class CaseworkerDataResponseModel
    {
        //[JsonProperty("id")]
        public string CaseworkerId { get; }

        //[JsonProperty("name")]
        public string DisplayName { get; }

        //[JsonProperty("givenName")]
        public string GivenName { get; }

        //[JsonProperty("middleName")]
        public string MiddleName { get; }

        //[JsonProperty("initials")]
        public string Initials { get; }

        //[JsonProperty(PropertyName = "email/address")]
        public Email Email { get; }

        //[JsonProperty("number")]
        public Phone Phone { get; }

        //[JsonProperty("caseworkerIdentifier")]
        public string CaseworkerIdentifier { get; }

        //[JsonProperty("description")]
        public string Description { get; }

        //[JsonProperty("isBookable")]
        public bool IsBookable { get; }

        //[JsonProperty("isActive")]
        public bool IsActive { get; }

        public CaseworkerDataResponseModel(string caseworkerId, string displayName, string givenName, string middleName, string initials, Email email, Phone phone,
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
}

