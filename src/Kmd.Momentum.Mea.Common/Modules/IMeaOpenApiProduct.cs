using System;

namespace Kmd.Momentum.Mea.Common.Modules
{
    public interface IMeaOpenApiProduct
    {
        string OpenApiProductName { get; }
        Version OpenApiVersion { get; }
        string ProductPathName { get; }
        bool IsControllerPartOfProduct(Type controllerType);
    }
}