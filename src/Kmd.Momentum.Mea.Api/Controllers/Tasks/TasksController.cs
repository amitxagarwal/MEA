using Kmd.Momentum.Mea.Common.Authorization;
using Kmd.Momentum.Mea.TaskApi;
using Kmd.Momentum.Mea.TaskApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api.Controllers.Tasks
{
    /// <summary>
    /// Controller to handle all the task related API requests
    /// </summary>
    [ApiController]
    [Route("tasks")]
    [Produces("application/json", "text/json")]
    [Authorize(MeaCustomClaimAttributes.TaskRole)]
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
        ///Update Status
        ///</summary>
        ///<response code="200">The task status is updated successfully</response>
        ///<response code="400">Bad request</response>
        ///<response code="404">The task details by id is not found</response>
        ///<response code="401">Couldn't get authorization to access Momentum Core Api</response>
        ///<param name="taskId">The TaskId to update the task in the Core system</param>
        ///<param name="taskUpdateStatus">The request model to update task in the Core system</param>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [Route("{taskId}/update")]
        [SwaggerOperation(OperationId = "Update Task status")]
        public async Task<ActionResult<TaskData>> UpdateTaskStatusById([Required] [FromRoute] string taskId, [Required] [FromBody] TaskUpdateStatus taskUpdateStatus)
        {
            var result = await _taskService.UpdateTaskStatusByIdAsync(taskId, taskUpdateStatus).ConfigureAwait(false);

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
