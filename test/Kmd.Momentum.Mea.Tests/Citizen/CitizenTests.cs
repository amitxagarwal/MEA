using FluentAssertions;
using Kmd.Momentum.Mea.Api;
using Kmd.Momentum.Mea.Api.Citizen;
using Kmd.Momentum.Mea.Api.Common;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Tests.Citizen
{
    public class CitizenTests
    {
        [Fact]
        public async Task GetAllActiveCitizensSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHelperHttpClient>();
            var cprArray = new string[] { "1234", "12345" };
            var _configurationRoot = new Mock<IConfiguration>();
            _configurationRoot.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com");

#pragma warning disable CA2000 // Dispose objects before losing scope
            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri($"{_configurationRoot.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/withActiveClassification"))).Returns(Task.FromResult(cprArray));
#pragma warning restore CA2000 // Dispose objects before losing scope

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configurationRoot.Object);

            //Act
            var result = await citizenService.GetAllActiveCitizensAsync().ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(cprArray);
        }

        [Fact]
        public async Task GetCitizenByCprSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHelperHttpClient>();
            var httpClientCitizenDataResponse = "{\"cpr\":\"dummyCpr\",\"id\":\"test-test-test-test-test\",\"displayName\":\"test display name\",\"" +
                "contactInformation\":{\"email\":{\"id\":\"testId-testId-testId-testId-testId\",\"address\":\"test@test.com\"},\"phone\":{\"id\":\"testId-testId-testId-testId-testId\",\"number\":\"+99999999\",\"isMobile\":true}}}";

            var citizenDataResponse = new CitizenDataResponse("test-test-test-test-test", "test display name", "", "", "", "test@test.com", "+99999999", "", "");
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
            var helperHttpClientMoq = new Mock<IHelperHttpClient>();
            var httpClientCitizenDataResponse = "{\"cpr\":\"dummyCpr\",\"id\":\"test-test-test-test-test\",\"displayName\":\"test display name\",\"" +
                "contactInformation\":{\"email\":{\"id\":\"testId-testId-testId-testId-testId\",\"address\":\"test@test.com\"},\"phone\":{\"id\":\"testId-testId-testId-testId-testId\",\"number\":\"+99999999\",\"isMobile\":true}}}";

            var citizenDataResponse = new CitizenDataResponse("test-test-test-test-test", "test display name", "", "", "", "test@test.com", "+99999999", "", "");
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
