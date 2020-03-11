﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    public interface ICitizenService
    {
        Task<string[]> GetAllActiveCitizens();

        Task<string[]> GetCitizenById(Guid citizenId);
        
    }
}
