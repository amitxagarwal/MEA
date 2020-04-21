using Kmd.Momentum.Mea.Caseworker;
using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Controllers.Caseworker
{
    [ApiController]
    [Route("caseworkers")]
    [Produces("application/json", "text/json")]
    [Authorize(MeaCustomClaimAttributes.CaseworkerRole)]
    public class CaseworkerController : ControllerBase
    {
        private readonly ICaseworkerService _caseworkerService;

        public CaseworkerController(ICaseworkerService caseworkerService)
        {
            _caseworkerService = caseworkerService ?? throw new ArgumentNullException(nameof(caseworkerService));
        }

        ///<summary>
        ///Get all the  caseworkers
        ///</summary>
        ///<response code="200">All the caseworkers data is loaded successfully</response>
        ///<response code="400"> Bad request</response>
        ///<response code="404">The caseworker data is not found</response>
        ///<response code="401">Couldn't get authorization to access Momentum Core Api</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<MeaBaseList>> GetAllCaseworkers([Required] [FromQuery] int pageNumber = 0)
        {
            var result = await _caseworkerService.GetAllCaseworkersAsync(pageNumber).ConfigureAwait(false);

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
        ///Get caseworkers in Momentum with ID
        ///</summary>
        ///<response code="200">The caseworker detail by id is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">The caseworker detail by id is not found</response>
        ///<response code="401">Couldn't get authorization to access Momentum Core Api</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Route("kss/{caseworkerId}")]
        [SwaggerOperation(OperationId = "getCaseworkerById")]
        public async Task<ActionResult<McaCaseworkerDataResponse>> GetCaseworkerById([Required] [FromRoute] string caseworkerId)
        {
            var result = await _caseworkerService.GetCaseworkerByIdAsync(caseworkerId).ConfigureAwait(false);

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

