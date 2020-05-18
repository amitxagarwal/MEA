using FluentAssertions;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Kmd.Momentum.Mea.TaskApi;
using Kmd.Momentum.Mea.TaskApi.Model;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Tests.Tasks
{
    public class TaskTests
    {
        private Mock<IHttpContextAccessor> GetContext()
        {
            var context = new Mock<IHttpContextAccessor>();
            var hc = new DefaultHttpContext();
            hc.TraceIdentifier = Guid.NewGuid().ToString();
            var claims = new List<Claim>()
                        {
                            new Claim("azp", Guid.NewGuid().ToString()),
                        };
            var identity = new ClaimsIdentity(claims, "JWT");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            hc.User = claimsPrincipal;
            context.Setup(x => x.HttpContext).Returns(hc);
            return context;
        }

        [Fact]
        public async Task UpdateTaskStatusSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ITaskHttpClientHelper>();
            var taskId = It.IsAny<Guid>();
            var taskStateValue = It.IsAny<int>();
            var context = GetContext();

            var response = new TaskDataResponseModel(taskId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
               It.IsAny<TaskState>(), It.IsAny<IReadOnlyList<AssignedActors>>(), It.IsAny<Reference>());

            var responseData = JsonConvert.SerializeObject(response);


            var taskUpdateStatus = new TaskUpdateStatus()
            {
                TaskAction = TaskAction.Completed,
                TaskContext = TaskContext.Citizens
            };

            helperHttpClientMoq.Setup(x => x.UpdateTaskStatusByTaskIdFromMomentumCoreAsync($"/tasks/{taskId}/{taskStateValue}?applicationContext={TaskContext.Citizens}"))
                   .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(responseData)));

            var taskService = new TaskService(helperHttpClientMoq.Object, context.Object);

            //Act
            var result = await taskService.UpdateTaskStatusByIdAsync(taskId.ToString(), taskUpdateStatus).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task UpdateTaskStatusFails()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ITaskHttpClientHelper>();
            var taskId = It.IsAny<Guid>();
            var taskStateValue = It.IsAny<int>();
            var context = GetContext();

            var response = new TaskDataResponseModel(taskId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
               It.IsAny<TaskState>(), It.IsAny<IReadOnlyList<AssignedActors>>(), It.IsAny<Reference>());

            var responseData = JsonConvert.SerializeObject(response);


            var taskUpdateStatus = new TaskUpdateStatus()
            {
                TaskAction = TaskAction.Completed,
                TaskContext = TaskContext.Citizens
            };

            var error = new Error("123456", new string[] { "Some error occured while updating task status" }, "MCA");

            helperHttpClientMoq.Setup(x => x.UpdateTaskStatusByTaskIdFromMomentumCoreAsync($"/tasks/{taskId}/{taskStateValue}?applicationContext={TaskContext.Citizens}"))
                   .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(error, HttpStatusCode.BadRequest)));

            var taskService = new TaskService(helperHttpClientMoq.Object, context.Object);

            //Act
            var result = await taskService.UpdateTaskStatusByIdAsync(taskId.ToString(), taskUpdateStatus).ConfigureAwait(false);

            //Asert
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().BeEquivalentTo("Some error occured while updating task status");
        }
    }
}
