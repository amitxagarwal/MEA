using FluentAssertions;
using Kmd.Momentum.Mea.Citizen;
using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Tests.Citizen
{
    public class CitizenTests
    {
        [Fact]
        public async Task GetAllActiveCitizensSuccess()
        {
            var helperHttpClientMoq = new Mock<IHttpClientHelper>();

             var _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            var mockResponseData = new List<string>();

            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId1", "TestDisplay1","givenname","middlename","initials","test@email.com","1234567891","","description",true,true)));
            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId2", "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));
            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}/search"))).Returns(Task.FromResult((IReadOnlyList<string>)mockResponseData));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);
            var responseData = mockResponseData.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x)).ToList();

            //Act
            var result = await citizenService.GetAllActiveCitizensAsync().ConfigureAwait(false);
            
            
            //Asert
            result.Should().NotBeNull();
            result.Result.Should().BeEquivalentTo(cprArray);
        }

        [Fact]
        public async Task GetCitizenByCprSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHttpClientHelper>();
            var _configuration = new Mock<IConfiguration>();

            var httpClientCitizenDataResponse = "{\"cpr\":\"dummyCpr\",\"id\":\"test-test-test-test-test\"," +
                "\"displayName\":\"test display name\",\"" +
                "contactInformation\":{\"email\":{\"id\":\"testId-testId-testId-testId-testId\"," +
                "\"address\":\"test@test.com\"},\"phone\":{\"id\":\"testId-testId-testId-testId-testId\"," +
                "\"number\":\"+99999999\",\"isMobile\":true}}}";

            var citizenDataResponse = new CitizenDataResponseModel("test-test-test-test-test", "test display name", 
                "", "", "", "test@test.com", "+99999999", "", "");
           
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/dummyCpr")))
                .Returns(Task.FromResult(new ResultOrHttpError<string, bool>(httpClientCitizenDataResponse)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByCprAsync("dummyCpr").ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(citizenDataResponse);
        }

        [Fact]
        public async Task GetCitizenDataByCprFails()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHttpClientHelper>();
            var _configuration = new Mock<IConfiguration>();

            var citizenDataResponse = new CitizenDataResponseModel("test-test-test-test-test", "test display name",
                "", "", "", "test@test.com", "+99999999", "", "");

            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/dummyCpr")))
                .Returns(Task.FromResult(new ResultOrHttpError<string, bool>(true, HttpStatusCode.NotFound)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByCprAsync("dummyCpr").ConfigureAwait(false);

            //Asert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("An Error Occured while retriving citizen data by cpr");
        }

        [Fact]
        public async Task GetCitizenByCitizenIdSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHttpClientHelper>();
            var _configuration = new Mock<IConfiguration>();

            var httpClientCitizenDataResponse = "{\"cpr\":\"dummyCpr\",\"id\":\"test-test-test-test-test\"," +
                "\"displayName\":\"test display name\",\"" +
                "contactInformation\":{\"email\":{\"id\":\"testId-testId-testId-testId-testId\"," +
                "\"address\":\"test@test.com\"},\"phone\":{\"id\":\"testId-testId-testId-testId-testId\"," +
                "\"number\":\"+99999999\",\"isMobile\":true}}}";

            var citizenDataResponse = new CitizenDataResponseModel("test-test-test-test-test", "test display name",
                "", "", "", "test@test.com", "+99999999", "", "");
            
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/dummyCitizenId")))
                .Returns(Task.FromResult(new ResultOrHttpError<string, bool>(httpClientCitizenDataResponse)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByIdAsync("testId1").ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.Result.Should().BeEquivalentTo(citizenDataResponse);
        }


        [Fact]
        public async Task GetCitizenByCitizenIdFails()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHttpClientHelper>();
            var _configuration = new Mock<IConfiguration>();

            var citizenDataResponse = new CitizenDataResponseModel("test-test-test-test-test", "test display name",
                "", "", "", "test@test.com", "+99999999", "", "");

            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/dummyCitizenId")))
                .Returns(Task.FromResult(new ResultOrHttpError<string, bool>(true, HttpStatusCode.NotFound)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByIdAsync("dummyCitizenId").ConfigureAwait(false);

            //Asert
            result.IsError.Should().BeTrue();
            result.Error.Should().Be("An Error Occured while retriving citizen data by citizenId");
        }
    }
}
