using Kmd.Momentum.Mea.Citizen;
using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Controllers.Citizen
{
    [ApiController]
    [Route("citizens")]
    [Produces("application/json", "text/json")]
    [Authorize(MeaCustomClaimAttributes.AudienceClaimTypeName)]
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
        public async Task<ActionResult<IReadOnlyList<CitizenDataResponseModel>>> GetAllActiveCitizens()
        {
            var result = await _citizenService.GetAllActiveCitizensAsync().ConfigureAwait(false);
            
            if (result.IsError)
            {
                return StatusCode((int)(result.StatusCode ?? HttpStatusCode.BadRequest), result.Error.Errors);
            }
            else
            {
                return Ok(result.Result);
            }
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
        public async Task<ActionResult<CitizenDataResponseModel>> GetCitizenByCpr([Required] [FromRoute] string cprNumber)
        {
            var result = await _citizenService.GetCitizenByCprAsync(cprNumber).ConfigureAwait(false);

            if (result.IsError)
            {
                return StatusCode((int)(result.StatusCode ?? HttpStatusCode.BadRequest), result.Error.Errors);
            }
            else 
            {
                return Ok(result.Result);
            }
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
        public async Task<ActionResult<CitizenDataResponseModel>> GetCitizenById([Required] [FromRoute] string citizenId)
        {
            var result = await _citizenService.GetCitizenByIdAsync(citizenId).ConfigureAwait(false);

            if (result.IsError)
            {
                return StatusCode((int)(result.StatusCode ?? HttpStatusCode.BadRequest), result.Error.Errors);
            }
            else
            {
                return Ok(result.Result);
            }
        }
    }
}
