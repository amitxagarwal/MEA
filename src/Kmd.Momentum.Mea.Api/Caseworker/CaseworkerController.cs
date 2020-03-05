using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Kmd.Momentum.Mea.Api
{
    [ApiController]
    [Route("")]
    [Produces("application/json", "text/json")]
    public class CaseworkerController : ControllerBase
    {
        private readonly ICaseworkerService _caseworkerService;

        public CaseworkerController(ICaseworkerService caseworkerService)
        {
            _caseworkerService = caseworkerService ?? throw new ArgumentNullException(nameof(caseworkerService));
        }

        ///<summary>
        ///Loads the data for Caseworker.
        ///</summary>
        ///<param name= "query"></param>
        ///<response code="200">The data is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">Document is not found</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("")]
        [SwaggerOperation(OperationId = "GetCaseworkerData")]
#pragma warning disable CA1822 // Mark members as static
        public async Task<CaseworkerResponse> GetCaseworkerDetailsAsync(Guid Id, CaseworkerRequest request)
#pragma warning restore CA1822 // Mark members as static
        {
            await Task.Delay(300).ConfigureAwait(false);
            return new CaseworkerResponse() { citizenName = "", document = "" };
        }
    }
}
