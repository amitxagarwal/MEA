using System;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    [Serializable]
    public class InvalidTenantException : Exception
    {
        public InvalidTenantException()
        {
        }

        public InvalidTenantException(string message) : base(message)
        {
        }
    }
}
