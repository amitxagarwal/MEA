using Kmd.Momentum.Mea.Common.KeyVault;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Controllers.Health
{
    /// <summary>
    /// Controllers to check the health of related APIs.
    /// </summary>
    [Route("[controller]/[action]")]
    //[Authorize]
    public class HealthController : Controller
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly IMeaSecretStore _meaSecretStore;

        /// <summary>
        /// Constructor for HealthCheckService
        /// </summary>
        /// <param name="healthCheckService"></param>
        /// <param name="meaSecretStore"></param>
        public HealthController(HealthCheckService healthCheckService, IMeaSecretStore meaSecretStore)
        {
            _healthCheckService = healthCheckService;
            _meaSecretStore = meaSecretStore;
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

        /// <summary>
        ///     Get secret
        /// </summary>
        /// <remarks>Provides an indication about the health of the API</remarks>
        /// <response code="200">API is healthy</response>
        /// <response code="503">API is unhealthy or in degraded state</response>
        [HttpGet]
        [ProducesResponseType(typeof(HealthReport), (int)HttpStatusCode.OK)]
        // [Authorize(Scopes.Access)]
        public async Task<ActionResult<string>> GetSecret([FromQuery] string secretIdentifier)
        {
            var result = await _meaSecretStore.GetSecretValueBySecretKeyAsync(secretIdentifier).ConfigureAwait(false);

            return Ok(result.SecretValue);
        }
    }
}