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


            var caseworkerData = new List<CaseworkerDataResponseModel>()
            {
                new CaseworkerDataResponseModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
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
            var id = "12345";

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

            //var email = new Email(id = "id", address = "address");
            
            //var caseworkerData = new CaseworkerDataResponse(id, "TestDisplay1", "givenname",
            //                "middlename", "initials", "", "description", true, true,null,null);
            //var httpClientCaseworkerDataResponse = JsonConvert.SerializeObject(caseworkerData);
            var responseData = new CaseworkerDataResponseModel("12345", "TestDisplay1", "givenname",
                            "middlename", "initials", "testemail", "123456", "", "description", true, true);

            helperHttpClientMoq.Setup(x => x.GetCaseworkerDataByCaseworkerIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}employees/{id}")))
                   .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(responseData)));

            var caseWorkerService = new CaseworkerService(helperHttpClientMoq.Object, _configuration.Object, context.Object);

            //Act
            var result = await caseWorkerService.GetCaseworkerByIdAsync(id).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(responseData);

        }

    }
}
