using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public class LogicOpenApiProduct : ILogicOpenApiProduct
    {
        private readonly Assembly[] assemblies;

        public LogicOpenApiProduct(string productPathName, string openApiProductName, Version openApiVersion, Assembly[] assemblies)
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

