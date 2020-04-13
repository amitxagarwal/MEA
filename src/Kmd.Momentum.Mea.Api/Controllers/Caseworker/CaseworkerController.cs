using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Caseworker1;
using Kmd.Momentum.Mea.Caseworker1.Model;
using Kmd.Momentum.Mea.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Controllers.Caseworker
{
    [ApiController]
    [Route("caseworker")]
    [Produces("application/json", "text/json")]
    [Authorize(MeaCustomClaimAttributes.AudienceClaimTypeName)]
    public class CaseworkerController : ControllerBase
    {
        private readonly ICaseworkerService _caseworkerService;

        public CaseworkerController(ICaseworkerService caseworkerService)
        {
            _caseworkerService = caseworkerService ?? throw new ArgumentNullException(nameof(caseworkerService));
        }

        ///<summary>
        ///Get all caseworkers in Momentum
        ///</summary>
        ///<response code="200">All the caseworker data in momemtum is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">The caseworker data is not found</response>
        ///<response code="401">Couldn't get authorization to access Momentum Core Api</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IReadOnlyList<ClaseworkerData>> GetAllCaseworkersInMomentum()
        {
            return await _caseworkerService.GetAllCaseworkersInMomentumAsync().ConfigureAwait(false);
        }
    }
}
