//using Kmd.Momentum.Mea.Common.Authorization;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Kmd.Momentum.Mea.Api.Controllers.Tasks
//{

//    /// <summary>
//    /// Controller to handle all the caseworker related API requests
//    /// </summary>
//    [ApiController]
//    [Route("caseworkers")]
//    [Produces("application/json", "text/json")]
//    [Authorize(MeaCustomClaimAttributes.CaseworkerRole)]
//    public class TasksController : ControllerBase
//    {
//        private readonly ITaskService _taskService;

//        /// <summary>
//        /// Constructor for caseworker controller
//        /// </summary>
//        /// <param name="caseworkerService"></param>
//        public TaskController(ITaskService taskService)
//        {
//            _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));
//        }
//    }
//}
