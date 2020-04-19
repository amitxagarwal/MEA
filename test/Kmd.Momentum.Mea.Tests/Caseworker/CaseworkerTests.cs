//using FluentAssertions;
//using Kmd.Momentum.Mea.Caseworker;
//using Kmd.Momentum.Mea.Caseworker.Model;
//using Kmd.Momentum.Mea.Common.Exceptions;
//using Kmd.Momentum.Mea.MeaHttpClientHelper;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Xunit;

//namespace Kmd.Momentum.Mea.Tests.Caseworker
//{
//    public class CaseworkerTests
//    {
//        [Fact]
//        public async Task GetAllCaseworkersSuccess()
//        {
//            //Arrange
//            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
//            var context = new Mock<IHttpContextAccessor>();
//            var hc = new DefaultHttpContext();
//            hc.TraceIdentifier = Guid.NewGuid().ToString();
//            var claims = new List<Claim>()
//                        {
//                            new Claim("azp", Guid.NewGuid().ToString()),
//                        };
//            var identity = new ClaimsIdentity(claims, "JWT");
//            var claimsPrincipal = new ClaimsPrincipal(identity);

//            hc.User = claimsPrincipal;

//            context.Setup(x => x.HttpContext).Returns(hc);

//            var _configuration = new Mock<IConfiguration>();
//            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

//            var mockResponseData = new List<CaseworkerDataResponseModel>()
//            {
//                    new CaseworkerDataResponseModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
//                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())
//            };

//            helperHttpClientMoq.Setup(x => x.GetAllCaseworkerDataFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers")))
//                    .Returns(Task.FromResult(new ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>(mockResponseData)));

//            var caseWorkerService = new CaseworkerService(helperHttpClientMoq.Object, _configuration.Object, context.Object);

//            //Act
//            var result = await caseWorkerService.GetAllCaseworkersAsync().ConfigureAwait(false);


//            //Asert
//            result.Should().NotBeNull();
//            result.IsError.Should().BeFalse();
//            result.Result.Should().BeEquivalentTo(mockResponseData);
//        }

//        [Fact]
//        public async Task GetAllCaseworkersFails()
//        {
//            //Arrange
//            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();
//            var context = new Mock<IHttpContextAccessor>();
//            var hc = new DefaultHttpContext();
//            hc.TraceIdentifier = Guid.NewGuid().ToString();
//            var claims = new List<Claim>()
//                        {
//                            new Claim("azp", Guid.NewGuid().ToString()),
//                        };
//            var identity = new ClaimsIdentity(claims, "JWT");
//            var claimsPrincipal = new ClaimsPrincipal(identity);

//            hc.User = claimsPrincipal;

//            context.Setup(x => x.HttpContext).Returns(hc);

//            var _configuration = new Mock<IConfiguration>();
//            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");


//            var mockResponseData = new List<CaseworkerDataResponseModel>()
//            {
//                    new CaseworkerDataResponseModel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
//                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())
//            };
//            var error = new Error("123456", new string[] { "An Error Occured while retriving data of all caseworkers" }, "MCA");

//            helperHttpClientMoq.Setup(x => x.GetAllCaseworkerDataFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers")))
//                    .Returns(Task.FromResult(new ResultOrHttpError<IReadOnlyList<CaseworkerDataResponseModel>, Error>(error, HttpStatusCode.BadRequest)));

//            var caseWorkerService = new CaseworkerService(helperHttpClientMoq.Object, _configuration.Object, context.Object);

//            //Act
//            var result = await caseWorkerService.GetAllCaseworkersAsync().ConfigureAwait(false);


//            //Asert
//            result.Should().NotBeNull();
//            result.IsError.Should().BeTrue();
//            result.Error.Errors[0].Should().Be("An Error Occured while retriving data of all caseworkers");
//        }

//    }
//}
