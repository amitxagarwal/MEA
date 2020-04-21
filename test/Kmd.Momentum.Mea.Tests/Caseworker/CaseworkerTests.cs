using FluentAssertions;
using Kmd.Momentum.Mea.Caseworker;
using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
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
        [Fact]
        public async Task GetAllCaseworkersSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
            var context = new Mock<IHttpContextAccessor>();
            var _configuration = new Mock<IConfiguration>();
            var pageNumber = 1;

            var hc = new DefaultHttpContext();
            hc.TraceIdentifier = Guid.NewGuid().ToString();
            context.Setup(x => x.HttpContext).Returns(hc);

            var claims = new List<Claim>()
                        {
                            new Claim("azp", Guid.NewGuid().ToString()),
                        };
            var identity = new ClaimsIdentity(claims, "JWT");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            hc.User = claimsPrincipal;

            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");


            var caseworkerData = new List<MeaCaseworkerDataResponseModel>()
            {
                new MeaCaseworkerDataResponseModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())
            };

            var responseData = new MeaBaseList()
            {
                TotalNoOfPages = 1,
                TotalSearchCount = 1,
                PageNo = 1,
                Result = caseworkerData
            };

            helperHttpClientMoq.Setup(x => x.GetAllCaseworkerDataFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers"), pageNumber))
                    .Returns(Task.FromResult(new ResultOrHttpError<MeaBaseList, Error>(responseData)));

            var caseWorkerService = new CaseworkerService(helperHttpClientMoq.Object, _configuration.Object, context.Object);

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
            var context = new Mock<IHttpContextAccessor>();
            var _configuration = new Mock<IConfiguration>();
            var pageNumber = 0;

            var hc = new DefaultHttpContext();
            hc.TraceIdentifier = Guid.NewGuid().ToString();
            context.Setup(x => x.HttpContext).Returns(hc);

            var claims = new List<Claim>()
                        {
                            new Claim("azp", Guid.NewGuid().ToString()),
                        };
            var identity = new ClaimsIdentity(claims, "JWT");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            hc.User = claimsPrincipal;

            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            var error = new Error("123456", new string[] { "An Error Occured while retriving data of all caseworkers" }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetAllCaseworkerDataFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers"), pageNumber))
                    .Returns(Task.FromResult(new ResultOrHttpError<MeaBaseList, Error>(error, HttpStatusCode.BadRequest)));

            var caseWorkerService = new CaseworkerService(helperHttpClientMoq.Object, _configuration.Object, context.Object);

            //Act
            var result = await caseWorkerService.GetAllCaseworkersAsync(pageNumber).ConfigureAwait(false);

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
            var context = new Mock<IHttpContextAccessor>();
            var _configuration = new Mock<IConfiguration>();
            var id = It.IsAny<string>();

            var hc = new DefaultHttpContext();
            hc.TraceIdentifier = Guid.NewGuid().ToString();
            context.Setup(x => x.HttpContext).Returns(hc);

            var claims = new List<Claim>()
                        {
                            new Claim("azp", Guid.NewGuid().ToString()),
                        };
            var identity = new ClaimsIdentity(claims, "JWT");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            hc.User = claimsPrincipal;

            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            var response = new MeaCaseworkerDataResponseModel(id, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            var responseData = JsonConvert.SerializeObject(response);

            helperHttpClientMoq.Setup(x => x.GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}employees/{id}")))
                   .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(responseData)));

            var caseWorkerService = new CaseworkerService(helperHttpClientMoq.Object, _configuration.Object, context.Object);

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
            var context = new Mock<IHttpContextAccessor>();
            var _configuration = new Mock<IConfiguration>();
            var id = It.IsAny<string>();

            var hc = new DefaultHttpContext();
            hc.TraceIdentifier = Guid.NewGuid().ToString();
            context.Setup(x => x.HttpContext).Returns(hc);

            var claims = new List<Claim>()
                        {
                            new Claim("azp", Guid.NewGuid().ToString()),
                        };
            var identity = new ClaimsIdentity(claims, "JWT");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            hc.User = claimsPrincipal;

            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            var response = new MeaCaseworkerDataResponseModel(id, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());

            var error = new Error("123456", new string[] { "Caseworker data with the supplied caseworkerId is not found" }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}employees/{id}")))
                   .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(error, HttpStatusCode.BadRequest)));

            var caseWorkerService = new CaseworkerService(helperHttpClientMoq.Object, _configuration.Object, context.Object);

            //Act
            var result = await caseWorkerService.GetCaseworkerByIdAsync(id).ConfigureAwait(false);

            //Asert
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("Caseworker data with the supplied caseworkerId is not found");
        }

    }
}
