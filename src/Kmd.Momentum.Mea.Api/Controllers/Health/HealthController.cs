using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Controllers.Health
{
    /// <summary>
    /// Health Controller
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    [Produces("application/json", "text/json")]
    //[Authorize]
    public class HealthController : Controller
    {
        private readonly HealthCheckService _healthCheckService ;

        /// <summary>
        /// Health Controller Constructor
        /// </summary>
        /// <param name="healthCheckService"></param>
        public HealthController(HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        /// <summary>
        ///     Get Health
        /// </summary>
        /// <remarks>Provides an indication about the health of the API</remarks>
        /// <response code="200">API is healthy</response>
        /// <response code="503">API is unhealthy or in degraded state</response>
        [HttpGet]
        [ProducesResponseType(typeof(HealthReport), (int)HttpStatusCode.OK)]
        [SwaggerOperation(OperationId = "HealthReady")]
        // [Authorize(Scopes.Access)]
        public async Task<IActionResult> Ready()
        {
            var report = await _healthCheckService.CheckHealthAsync().ConfigureAwait(true);

            return report.Status == HealthStatus.Healthy ? Ok(report) : StatusCode((int)HttpStatusCode.ServiceUnavailable, report);
        }

        /// <summary>
        ///     Get Health
        /// </summary>
        /// <remarks>Provides an indication about the health of the API</remarks>
        /// <response code="200">API is healthy</response>
        /// <response code="503">API is unhealthy or in degraded state</response>
        [HttpGet]
        [ProducesResponseType(typeof(HealthReport), (int)HttpStatusCode.OK)]
        [SwaggerOperation(OperationId = "HealthLive")]
        // [Authorize(Scopes.Access)]
        public async Task<IActionResult> Live()
        {
            var report = await _healthCheckService.CheckHealthAsync().ConfigureAwait(true);

            return report.Status == HealthStatus.Healthy ? Ok(report) : StatusCode((int)HttpStatusCode.ServiceUnavailable, report);
        }
    }
}