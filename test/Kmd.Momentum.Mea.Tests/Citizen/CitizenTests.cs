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
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Tests.Citizen
{
    public class CitizenTests
    {
        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public async Task GetAllActiveCitizensSuccess()
        {
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();

             var _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            var mockResponseData = new List<string>();

            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId1", "TestDisplay1","givenname","middlename","initials","test@email.com","1234567891","","description",true,true)));
            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId2", "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));

            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}/search")))
                .Returns(Task.FromResult(new ResultOrHttpError<IReadOnlyList<string>, Error>(mockResponseData)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);
            var responseData = mockResponseData.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x)).ToList();

            //Act
            var result = await citizenService.GetAllActiveCitizensAsync().ConfigureAwait(false);
            
            
            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(responseData);
        }

        [Fact]
        public async Task GetCitizenByCprSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var _configuration = new Mock<IConfiguration>();
            var cpr = "1234567890";

            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            var citizenData = new CitizenDataResponseModel("testId1", "TestDisplay1",
                "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true);

            var httpClientCitizenDataResponse = JsonConvert.SerializeObject(citizenData);


            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{cpr}")))
                .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(httpClientCitizenDataResponse)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByCprAsync(cpr).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(citizenData);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public async Task GetCitizenDataByCprFails()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var _configuration = new Mock<IConfiguration>();

            var cpr = "1234567890";
            var citizenDataResponse = new CitizenDataResponseModel("test-test-test-test-test", "test display name",
                "", "", "", "test@test.com", "+99999999", "", "");

            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");
            var error = new Error("Citizen with the supplied cpr no is not found", HttpStatusCode.NotFound);

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{cpr}")))
                .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(error)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByCprAsync(cpr).ConfigureAwait(false);

            //Asert
            result.IsError.Should().BeTrue();
            result.Error.Message.Should().Be("Citizen with the supplied cpr no is not found");
        }

        [Fact]
        public async Task GetCitizenByCitizenIdSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var _configuration = new Mock<IConfiguration>();
            var citizenId = "1234567890";
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            var citizenData = new CitizenDataResponseModel(citizenId, "TestDisplay1", "givenname",
                "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true);
            var httpClientCitizenDataResponse = JsonConvert.SerializeObject(citizenData);

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}")))
                .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(httpClientCitizenDataResponse)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByIdAsync(citizenId).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(citizenData);
        }


        [Fact]
        public async Task GetCitizenByCitizenIdFails()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var _configuration = new Mock<IConfiguration>();
            var citizenId = "1234567890";

            var citizenDataResponse = new CitizenDataResponseModel(citizenId, "test display name",
                "", "", "", "test@test.com", "+99999999", "", "");

            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenId}")))
                .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(new Error("Citizen by citizen id not found", HttpStatusCode.NotFound))));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByIdAsync(citizenId).ConfigureAwait(false);

            //Asert
            result.IsError.Should().BeTrue();
            result.Error.Message.Should().Be("Citizen by citizen id not found");
        }
    }
}
