using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kmd.Momentum.Mea.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Produces("application/json", "text/json")]
    public class CaseworkerDataController : ControllerBase
    {

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public string Get()
        {
            return "Implementation pending";
        }
    }
}