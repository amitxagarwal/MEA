using System;

namespace Kmd.Momentum.Mea.Common.Modules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoScopedDIAttribute : Attribute
    {
        public Type AsInterface { get; set; }

        public AutoScopedDIAttribute()
        {
        }

        public AutoScopedDIAttribute(Type asInterface)
        {
            AsInterface = asInterface;
        }
    }
}