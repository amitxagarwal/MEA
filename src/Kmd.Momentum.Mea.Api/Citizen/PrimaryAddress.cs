using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class PrimaryAddress
    {
        public string Building { get; }
        public string City { get; }
        public string CoName { get; }
        public string End { get; }
        public string Floor { get; }
        public string Id { get; }
        public bool IsCurrent { get; }
        public bool IsPrimary { get; }
        public bool IsTemporary { get; }
        public string Latitude { get; }
        public string Longitude { get; }
        public string PostalCode { get; }
        public string Start { get; }
        public string Street { get; }
        public string Suite { get; }
        public Country Country { get; }

        public PrimaryAddress(string building, string city, string coName, string end,
            string floor, string id, bool isCurrent, bool isPrimary, bool isTemporary, string latitude, string longitude, string postalCode,
            string start, string street, string suite, Country country = null)
        {
            Building = building;
            City = city;
            CoName = coName;
            End = end;
            Floor = floor;
            Id = id;
            IsCurrent = isCurrent;
            IsPrimary = isPrimary;
            IsTemporary = isTemporary;
            Latitude = latitude;
            Longitude = longitude;
            PostalCode = postalCode;
            Start = start;
            Street = street;
            Suite = suite;
            Country = country;
        }
    }
}
