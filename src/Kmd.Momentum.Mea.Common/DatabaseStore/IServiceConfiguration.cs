using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Momentum.Mea.Common.DatabaseStore
{
    public interface IServiceConfiguration
    {
        void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}