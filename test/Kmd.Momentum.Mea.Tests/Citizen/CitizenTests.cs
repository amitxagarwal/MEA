using FluentAssertions;
using Kmd.Momentum.Mea.Api;
using Kmd.Momentum.Mea.Citizen;
using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.HttpProvider;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
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

            var citizenDataResponse = new CitizenDataResponseModel("test-test-test-test-test","123456", "test display name", "", "", "", "test@test.com", "+99999999", "", "");
            var _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            var mockResponseData = new List<string>();

            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId1", "TestCPR1", "TestDisplay1","givenname","middlename","initials","test@email.com","1234567891","","description")));
            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId2", "TestCPR2", "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description")));
            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}/citizensearch"))).Returns(Task.FromResult((IReadOnlyList<string>)mockResponseData));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);
            var responseData = mockResponseData.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x)).ToList();

            //Act
            var result = await citizenService.GetAllActiveCitizensAsync().ConfigureAwait(false);
            
            
            //Asert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(responseData);
        }

        [Fact]
        public async Task GetCitizenByCprSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHttpClientHelper>();
            var httpClientCitizenDataResponse = "{\"cpr\":\"dummyCpr\",\"id\":\"test-test-test-test-test\",\"displayName\":\"test display name\",\"" +
                "contactInformation\":{\"email\":{\"id\":\"testId-testId-testId-testId-testId\",\"address\":\"test@test.com\"},\"phone\":{\"id\":\"testId-testId-testId-testId-testId\",\"number\":\"+99999999\",\"isMobile\":true}}}";

            var citizenDataResponse = new CitizenDataResponseModel("test-test-test-test-test", "test display name", "", "", "", "test@test.com", "+99999999", "", "", "");
            var _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/dummyCpr"))).Returns(Task.FromResult(httpClientCitizenDataResponse));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByCprAsync("dummyCpr").ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(citizenDataResponse);
        }

        [Fact]
        public async Task GetCitizenByIdSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHttpClientHelper>();
            var httpClientCitizenDataResponse = "{\"cpr\":\"dummyCpr\",\"id\":\"test-test-test-test-test\",\"displayName\":\"test display name\",\"" +
                "contactInformation\":{\"email\":{\"id\":\"testId-testId-testId-testId-testId\",\"address\":\"test@test.com\"},\"phone\":{\"id\":\"testId-testId-testId-testId-testId\",\"number\":\"+99999999\",\"isMobile\":true}}}";

            var citizenDataResponse = new CitizenDataResponseModel("test-test-test-test-test", "test display name", "", "", "", "test@test.com", "+99999999", "","", "");
            var _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/dummyCitizenId"))).Returns(Task.FromResult(httpClientCitizenDataResponse));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByIdAsync("dummyCitizenId").ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(citizenDataResponse);
        }
    }
}
