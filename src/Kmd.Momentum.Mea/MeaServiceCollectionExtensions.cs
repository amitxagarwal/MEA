using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea
{
    public static class MeaServiceCollectionExtensions
    {
        public static IServiceCollection AddMea(this IServiceCollection services)
        {
            return services;
        }
    }
}
