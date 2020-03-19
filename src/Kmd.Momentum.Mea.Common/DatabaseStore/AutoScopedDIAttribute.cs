using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
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