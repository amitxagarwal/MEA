using System;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class DocumentMappableAttribute : Attribute
    {
        public string TypeName { get; }

        public DocumentMappableAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }
}