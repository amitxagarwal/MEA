using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    [ApiController]
    [Route("citizen")]
    [Produces("application/json", "text/json")]
    public class CitizenController : ControllerBase
    {
        private readonly ICitizenService _citizenService;

        public CitizenController(ICitizenService caseworkerService)
        {
            _citizenService = caseworkerService ?? throw new ArgumentNullException(nameof(caseworkerService));
        }

        ///<summary>
        ///Loads the data for Caseworker.
        ///</summary>
        ///<response code="200">The data is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">Document is not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("/withActiveClassification")]
        [SwaggerOperation(OperationId = "GetAllActiveCitizens")]
        public async Task<int[]> GetAllActiveCitizens()
        {
            return await _citizenService.GetAllActiveCitizens().ConfigureAwait(false);
        }

    }
}
