using System;
using System.Linq;
using System.Reflection;

namespace Kmd.Momentum.Mea.Common.Modules
{
    public class MeaOpenApiProduct : IMeaOpenApiProduct
    {
        private readonly Assembly[] assemblies;

        public MeaOpenApiProduct(string productPathName, string openApiProductName, Version openApiVersion, Assembly[] assemblies)
        {
            ProductPathName = productPathName; // null means "no product"
            OpenApiProductName = openApiProductName; // Null means "no product"
            OpenApiVersion = openApiVersion; // Null means "no version"

            this.assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        public string OpenApiProductName { get; }
        public Version OpenApiVersion { get; }
        public string ProductPathName { get; }

        public bool IsControllerPartOfProduct(Type controllerType) => assemblies.Any(a => a == controllerType.Assembly);
    }
}

