using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface ILogicOpenApiProduct
    {
        string OpenApiProductName { get; }
        Version OpenApiVersion { get; }
        string ProductPathName { get; }
        bool IsControllerPartOfProduct(Type controllerType);
    }
}