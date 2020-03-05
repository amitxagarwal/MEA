using System;

namespace Kmd.Momentum.Mea.Api
{
    public class CaseworkerData
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Municipality { get; }

        public CaseworkerData(Guid id, string name, string municipality)
        {
            Id = id;
            Name = name;
            Municipality = municipality;
        }
    }
}