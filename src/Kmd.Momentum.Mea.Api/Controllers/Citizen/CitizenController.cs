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
    /// <summary>
    /// Controller to handle all the Citizen related API requests
    /// </summary>
    [ApiController]
    [Route("citizens")]
    [Produces("application/json", "text/json")]
    [Authorize(MeaCustomClaimAttributes.CitizenRole)]
    public class CitizenController : ControllerBase
    {
        private readonly ICitizenService _citizenService;

        /// <summary>
        /// Constructor for CitizenController
        /// </summary>
        /// <param name="citizenService"></param>
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
        ///<param name="pageNumber">The PageNumber to access the records from Core system. Minimum Value is 1</param>
        ///<returns>List of CitizenDataResponseModel</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [SwaggerOperation(OperationId = "GetAllActiveCitizens")]
        public async Task<ActionResult<IReadOnlyList<CitizenDataResponseModel>>> GetAllActiveCitizens([FromQuery] [Required] int pageNumber)
        {
            var result = await _citizenService.GetAllActiveCitizensAsync(pageNumber).ConfigureAwait(false);
            
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
        ///<param name="cprNumber">The CPR number to search the record in the Core system</param>
        ///<returns>CitizenDataResponseModel</returns>
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
        ///<param name="citizenId">The Citizen ID or Momentum Id to search the record in the Core system</param>
        ///<returns>CitizenDataResponseModel</returns>
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
        ///Create a Journal Note with attachment
        ///</summary>
        ///<response code="200">The Journal Note is created successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">The Journal Note to create is not found</response>
        ///<response code="401">Couldn't get authorization to access Momentum Core Api</response>
        ///<param name="momentumCitizenId">The MomentumCitizenID or CitizenId to Create the journal note record in the Core system</param>
        ///<param name="requestModel">The requestmodel to save as a journal note record, in the Core system</param>
        ///<returns>Ok</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Route("journal/{momentumCitizenId}")]
        public async Task<ActionResult> CreateJournalNote([Required] [FromRoute] string momentumCitizenId, [Required] [FromBody] JournalNoteResponseModel requestModel)
        {
            var result = await _citizenService.CreateJournalNoteAsync(momentumCitizenId, requestModel).ConfigureAwait(false);

            if (result.IsError)
            {
                return StatusCode((int)(result.StatusCode ?? HttpStatusCode.BadRequest), result.Error.Errors);
            }
            else
            {
                return Ok("OK");
            }
        }
    }
}
