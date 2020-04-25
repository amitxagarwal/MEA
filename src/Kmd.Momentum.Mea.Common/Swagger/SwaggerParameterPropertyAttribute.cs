using System;

namespace Kmd.Momentum.Mea.Common.Swagger
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class SwaggerParameterPropertyAttribute : Attribute
    {
        public SwaggerParameterPropertyAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string DataType { get; set; }
        public string ParameterType { get; set; }
        public string Description { get; private set; }
        public bool Required { get; set; } = false;
    }
}
