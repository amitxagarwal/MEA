using FluentAssertions;
using Kmd.Momentum.Mea.Caseworker;
using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Authorization;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Kmd.Momentum.Mea.TaskApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
                            new Claim("tenant", "159"),
                        };
            var identity = new ClaimsIdentity(claims, "JWT");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            hc.User = claimsPrincipal;
            context.Setup(x => x.HttpContext).Returns(hc);
            return context;
        }

        private IConfiguration GetConifg()
        {
            var meaAuthList = new List<MeaAuthorization>();

            meaAuthList.Add(new MeaAuthorization()
            {
                KommuneId = "159",
                KommuneUrl = "0d1345f4-51e0-407e-9dc0-15a9d08326d7",
                KommuneClientId = "4a7a4c73-f203-435e-b5c2-3cbba12f0285",
                KommuneAccessIdentifier = "ClientSecret",
                KommuneResource = "74b4f45c-4e9b-4be1-98f1-ea876d9edd11",
                PunitId = "0d1345f4-51e0-407e-9dc0-15a9d08326d7"
            });

            IReadOnlyList<MeaAuthorization> _meaAuthList = meaAuthList;
            var _meaObj = new
            {
                MeaAuthorization = _meaAuthList
            };

            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_meaObj)));

            IConfiguration _config = new ConfigurationBuilder().AddJsonStream(memoryStream).Build();

            return _config;

        }

        [Fact]
        public async Task GetAllCaseworkersSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            var pageNumber = 1;
            var context = GetContext();
            var _config = GetConifg();

            var caseworkerData = new List<CaseworkerDataResponseModel>()
                {
                    new CaseworkerDataResponseModel(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
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

            var caseWorkerService = new CaseworkerService(_config, helperHttpClientMoq.Object, context.Object);

            //Act
            var result = await caseWorkerService.GetAllCaseworkersAsync(pageNumber).ConfigureAwait(false);

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
            var pageNumber = 0;
            var context = GetContext();
            var _config = GetConifg();

            var error = new Error("123456", new string[] { "An Error Occured while retriving data of all caseworkers" }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetAllCaseworkerDataFromMomentumCoreAsync("/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers", pageNumber))
                    .Returns(Task.FromResult(new ResultOrHttpError<CaseworkerList, Error>(error, HttpStatusCode.BadRequest)));

            var caseWorkerService = new CaseworkerService(_config, helperHttpClientMoq.Object, context.Object);

            //Act
            var result = await caseWorkerService.GetAllCaseworkersAsync(pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("An Error Occured while retriving data of all caseworkers");
        }

        [Fact]
        public async Task GetAllCaseworkersFailsWhenPAgeNoIsLessThan0()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            int pageNumber = -3;
            var context = GetContext();
            var _config = GetConifg();

            var error = new Error("123456", new string[] { "The offset specified in a OFFSET clause may not be negative." }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetAllCaseworkerDataFromMomentumCoreAsync("/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers", pageNumber))
                    .Returns(Task.FromResult(new ResultOrHttpError<CaseworkerList, Error>(error, HttpStatusCode.BadRequest)));

            var caseWorkerService = new CaseworkerService(_config, helperHttpClientMoq.Object, context.Object);

            //Act
            var result = await caseWorkerService.GetAllCaseworkersAsync(pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("The offset specified in a OFFSET clause may not be negative.");
        }

        [Fact]
        public async Task GetCaseworkerByCaseworkerIdSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            var id = It.IsAny<string>();
            var context = GetContext();
            var _config = GetConifg();

            var response = new CaseworkerDataResponseModelBuilder().Build();

            var responseData = JsonConvert.SerializeObject(response);

            helperHttpClientMoq.Setup(x => x.GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync($"employees/{id}"))
                   .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(responseData)));

            var caseWorkerService = new CaseworkerService(_config, helperHttpClientMoq.Object, context.Object);

            //Act
            var result = await caseWorkerService.GetCaseworkerByIdAsync(id).ConfigureAwait(false);

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
            var id = It.IsAny<string>();
            var context = GetContext();
            var _config = GetConifg();

            var response = new CaseworkerDataResponseModelBuilder().Build();

            var error = new Error("123456", new string[] { "Caseworker data with the supplied caseworkerId is not found" }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync($"employees/{id}"))
                   .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(error, HttpStatusCode.BadRequest)));

            var caseWorkerService = new CaseworkerService(_config, helperHttpClientMoq.Object, context.Object);

            //Act
            var result = await caseWorkerService.GetCaseworkerByIdAsync(id).ConfigureAwait(false);

            //Asert
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("Caseworker data with the supplied caseworkerId is not found");
        }

        [Fact]
        public async Task GetAllTasksByCaseworkerIdSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            var id = It.IsAny<Guid>();
            var pageNumber = 1;
            var caseworkerId = It.IsAny<string>();
            var context = GetContext();
            var _config = GetConifg();

            var TaskData = new List<TaskDataResponseModel>()
            {
               new TaskDataResponseModel (id, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(),
               It.IsAny<TaskState>(), It.IsAny<IReadOnlyList<AssignedActors>>(), It.IsAny<Reference>())
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

            var caseworkerService = new CaseworkerService(_config, helperHttpClientMoq.Object, context.Object);

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
            var id = It.IsAny<string>();
            var pageNumber = 1;
            var caseworkerId = It.IsAny<string>();
            var context = GetContext();
            var _config = GetConifg();

            var error = new Error("123456", new string[] { "An Error Occured while retriving tasks with given caseworkerId" }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetAllTasksByCaseworkerIdFromMomentumCoreAsync("/tasks/filtered", pageNumber, caseworkerId))
                    .Returns(Task.FromResult(new ResultOrHttpError<TaskList, Error>(error, HttpStatusCode.BadRequest)));

            var caseworkerService = new CaseworkerService(_config, helperHttpClientMoq.Object, context.Object);

            //Act
            var result = await caseworkerService.GetAllTasksForCaseworkerIdAsync(caseworkerId, pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("An Error Occured while retriving tasks with given caseworkerId");
        }
    }

}

