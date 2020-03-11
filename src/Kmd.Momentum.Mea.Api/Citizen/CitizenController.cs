using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
       

        public CitizenController(ICitizenService citizenService)
        {
            _citizenService = citizenService ?? throw new ArgumentNullException(nameof(citizenService));            
        }

        ///<summary>
        ///Loads the data for Citizen.
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
        ///Get Citizens in Momentum with ID
        ///</summary>
        ///<response code="200">The data is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">Document is not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("/citizenById")]
        [SwaggerOperation(OperationId = "GetCitizenById")]
        public async Task<string[]> GetCitizenById()
        {
            return await _citizenService.GetCitizenById(new Guid("7b51f7e4-c46e-4494-95e8-13fb8098cfd8")).ConfigureAwait(false);

        }

    }
}
