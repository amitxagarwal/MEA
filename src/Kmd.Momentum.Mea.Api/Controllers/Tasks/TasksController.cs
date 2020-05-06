using Kmd.Momentum.Mea.Common.Authorization;
using Kmd.Momentum.Mea.TaskApi;
using Kmd.Momentum.Mea.TaskApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Controllers.Tasks
{

    /// <summary>
    /// Controller to handle all the caseworker related API requests
    /// </summary>
    [ApiController]
    [Route("tasks")]
    [Produces("application/json", "text/json")]
    [Authorize(MeaCustomClaimAttributes.CaseworkerRole)]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// Constructor for task controller
        /// </summary>
        /// <param name="taskService"></param>
    
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
        }

        ///<summary>
        ///Get caseworkers in Momentum with ID
        ///</summary>
        ///<response code="200">The caseworker detail by id is loaded successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">The caseworker detail by id is not found</response>
        ///<response code="401">Couldn't get authorization to access Momentum Core Api</response>
        ///<param name="id">The caseworker id to access the records from Core system.</param>
        ///<returns>caseworker details</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Route("{taskID}/update")]
        [SwaggerOperation(OperationId = "getCaseworkerById")]
        public async Task<ActionResult<TaskData>> GetCaseworkerById([Required] [FromRoute] string taskID, [Required] [FromBody] TaskUpdateModel taskUpdateStatus)
        {
            var result = await _taskService.UpdateTaskStatusByIdAsync(taskID, taskUpdateStatus).ConfigureAwait(false);

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
