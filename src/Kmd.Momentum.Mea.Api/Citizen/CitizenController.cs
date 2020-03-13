using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Citizen
{
    [ApiController]
    [Route("citizen")]
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
        ///<response code="200">The data is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">Document is not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("/withActiveClassification")]
        [SwaggerOperation(OperationId = "GetAllActiveCitizens")]
        public async Task<string[]> GetAllActiveCitizens()
        {
            return await _citizenService.GetAllActiveCitizens().ConfigureAwait(false);
        }

        ///<summary>
        ///Get Citizens in Momentum with CPR
        ///</summary>
        ///<response code="200">The data is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">Document is not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("/citizenByCpr/{cpr}")]
        [SwaggerOperation(OperationId = "GetCitizenByCpr")]
        public async Task<CitizenDataResponse> GetCitizenByCpr([Required] [FromRoute] string cpr)
        {
            return await _citizenService.getCitizenByCpr(cpr).ConfigureAwait(false);
        }

        ///<summary>
        ///Get Citizens in Momentum with ID
        ///</summary>
        ///<response code="200">The data is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">Document is not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("/citizenById/{citizenId}")]
        [SwaggerOperation(OperationId = "getCitizenById")]
        public async Task<CitizenDataResponse> GetCitizenById([Required] [FromRoute] string citizenId)
        {
            var response = await _citizenService.getCitizenById(citizenId).ConfigureAwait(false);
            return response;

        }

    }
}
