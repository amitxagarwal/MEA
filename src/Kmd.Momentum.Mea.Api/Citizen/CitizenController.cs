using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    [ApiController]
    [Route("citizens")]
    [Produces("application/json", "text/json")]
    public class CitizenController : ControllerBase
    {
        private readonly ICitizenService _citizenService;

        public CitizenController(ICitizenService citizenService)
        {
            _citizenService = citizenService ?? throw new ArgumentNullException(nameof(citizenService));
        }

        ///<summary>
        ///Get all active citizens
        ///</summary>
        ///<response code="200">The active citizen data is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">The active citizen data is not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [SwaggerOperation(OperationId = "GetAllActiveCitizens")]
        public async Task<IReadOnlyList<Data>> GetAllActiveCitizens()
        {
            return await _citizenService.GetAllActiveCitizensAsync().ConfigureAwait(false);
        }

        ///<summary>
        ///Get Citizen in Momentum by CPR
        ///</summary>
        ///<response code="200">The citizen detail by CPR no is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">The citizen detail by CPR no is not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("cpr/{cprNumber}")]
        [SwaggerOperation(OperationId = "GetCitizenByCpr")]
        public async Task<CitizenDataResponse> GetCitizenByCpr([Required] [FromRoute] string cprNumber)
        {
            return await _citizenService.GetCitizenByCprAsync(cprNumber).ConfigureAwait(false);
        }

        ///<summary>
        ///Get Citizen in Momentum by ID
        ///</summary>
        ///<response code="200">The citizen detail by id is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">The citizen detail by id is not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("kss/{citizenId}")]
        [SwaggerOperation(OperationId = "GetCitizenById")]
        public async Task<CitizenDataResponse> GetCitizenById([Required] [FromRoute] string citizenId)
        {
            var response = await _citizenService.GetCitizenByIdAsync(citizenId).ConfigureAwait(false);
            return response;
        }
    }
}
