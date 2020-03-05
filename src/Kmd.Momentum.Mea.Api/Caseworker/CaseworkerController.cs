using System;

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
        public async Task<ActionResult<CaseworkerData>> LoadCaseworkerData([Required] [FromRoute] String query)
        {
            var result = await _caseworkerService.LoadCaseworkerDataAsync(query).ConfigureAwait(false);

            if (result.IsError)
            {
                return StatusCode((int)(result.StatusCode ?? HttpStatusCode.BadRequest), result.Error);
            }
            else
            {
                return Ok(result.Result);
            }
        }
    }
}
