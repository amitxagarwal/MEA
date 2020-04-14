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
    [Authorize(MeaCustomClaimAttributes.CitizenRole)]
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
        ///<response code="401">Couldn't get authorization to access Momentum Core Api</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
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
        ///<response code="401">Couldn't get authorization to access Momentum Core Api</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
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
        ///<response code="401">Couldn't get authorization to access Momentum Core Api</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
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

        ///<summary>
        ///Create a Journal Note in Momentum with attachment
        ///</summary>
        ///<response code="200">The Journal Note created successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">The Create a Journal Note is not found</response>
        ///<response code="401">Couldn't get authorization to access Momentum Core Api</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Route("journal/{momentumCitizenId}")]
        public async Task<ActionResult> CreateJournalNote([Required] [FromRoute] string momentumCitizenId, [Required] [FromBody] MeaCitizenJournalNoteRequestModel requestModel)
        {
            var result = await _citizenService.CreateJournalNoteAsync(momentumCitizenId, requestModel).ConfigureAwait(false);

            if (result.IsError)
            {
                return StatusCode((int)(result.StatusCode ?? HttpStatusCode.BadRequest), result.Error.Errors);
            }
            else
            {
                return Ok();
            }
        }
    }
}
