using FluentAssertions;
using Kmd.Momentum.Mea.Caseworker;
using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Kmd.Momentum.Mea.TaskApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Tests.Caseworker
{
    public class CaseworkerTests
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
        public async Task GetAllCaseworkersSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            var configurationMoq = new Mock<IConfiguration>();
            var pageNumber = 1;
            var context = GetContext();

            var caseworkerData = new List<CaseworkerDataResponseModel>()
            {
                new CaseworkerDataResponseModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())
            };

            var responseData = new CaseworkerList()
            {
                TotalNoOfPages = 1,
                TotalSearchCount = 1,
                PageNo = 1,
                Result = caseworkerData
            };

            helperHttpClientMoq.Setup(x => x.GetAllCaseworkerDataFromMomentumCoreAsync("/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers", pageNumber))
                    .Returns(Task.FromResult(new ResultOrHttpError<CaseworkerList, Error>(responseData)));

            var caseworkerService = new CaseworkerService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await caseworkerService.GetAllCaseworkersAsync(pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(responseData);
        }

        [Fact]
        public async Task GetAllCaseworkersFails()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            var configurationMoq = new Mock<IConfiguration>();
            var pageNumber = 0;
            var context = GetContext();

            var error = new Error("123456", new string[] { "An Error Occured while retriving data of all caseworkers" }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetAllCaseworkerDataFromMomentumCoreAsync("/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers", pageNumber))
                    .Returns(Task.FromResult(new ResultOrHttpError<CaseworkerList, Error>(error, HttpStatusCode.BadRequest)));

            var caseworkerService = new CaseworkerService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await caseworkerService.GetAllCaseworkersAsync(pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("An Error Occured while retriving data of all caseworkers");
        }

        [Fact]
        public async Task GetCaseworkerByCaseworkerIdSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            var configurationMoq = new Mock<IConfiguration>();
            var id = It.IsAny<string>();
            var context = GetContext();

            var response = new CaseworkerDataResponseModel(id, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            var responseData = JsonConvert.SerializeObject(response);

            helperHttpClientMoq.Setup(x => x.GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync($"employees/{id}"))
                   .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(responseData)));

            var caseworkerService = new CaseworkerService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await caseworkerService.GetCaseworkerByIdAsync(id).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetCaseworkerByCaseworkerIdFails()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            var configurationMoq = new Mock<IConfiguration>();
            var id = It.IsAny<string>();
            var context = GetContext();

            var response = new CaseworkerDataResponseModel(id, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            var error = new Error("123456", new string[] { "Caseworker data with the supplied caseworkerId is not found" }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync($"employees/{id}"))
                   .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(error, HttpStatusCode.BadRequest)));

            var caseworkerService = new CaseworkerService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await caseworkerService.GetCaseworkerByIdAsync(id).ConfigureAwait(false);

            //Asert
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("Caseworker data with the supplied caseworkerId is not found");
        }

        [Fact]
        public async Task GetAllTasksByCaseworkerIdSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            var configurationMoq = new Mock<IConfiguration>();
            var id = It.IsAny<string>();
            var pageNumber = 1;
            var caseworkerId = It.IsAny<string>();
            var context = GetContext();

            var TaskData = new List<TaskDataResponseModel>()
            {
               new TaskDataResponseModel (id, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
               It.IsAny<string>(), It.IsAny<IReadOnlyList<AssignedActors>>(), It.IsAny<Reference>())
               };

            var responseData = new TaskList()
            {
                TotalNoOfPages = 1,
                TotalSearchCount = 1,
                PageNo = 1,
                Result = TaskData
            };

            helperHttpClientMoq.Setup(x => x.GetAllTasksByCaseworkerIdFromMomentumCoreAsync("/tasks/filtered", pageNumber, caseworkerId))
                               .Returns(Task.FromResult(new ResultOrHttpError<TaskList, Error>(responseData)));

            var caseworkerService = new CaseworkerService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await caseworkerService.GetAllTasksForCaseworkerIdAsync(caseworkerId, pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(responseData);
        }

        [Fact]
        public async Task GetAllTasksByCaseworkerIdFails()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            var configurationMoq = new Mock<IConfiguration>();
            var id = It.IsAny<string>();
            var pageNumber = 1;
            var caseworkerId = It.IsAny<string>();
            var context = GetContext();

            var error = new Error("123456", new string[] { "An Error Occured while retriving tasks with given caseworkerId" }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetAllTasksByCaseworkerIdFromMomentumCoreAsync("/tasks/filtered", pageNumber, caseworkerId))
                    .Returns(Task.FromResult(new ResultOrHttpError<TaskList, Error>(error, HttpStatusCode.BadRequest)));

            var caseworkerService = new CaseworkerService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await caseworkerService.GetAllTasksForCaseworkerIdAsync(caseworkerId, pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("An Error Occured while retriving tasks with given caseworkerId");
        }
    }

}

