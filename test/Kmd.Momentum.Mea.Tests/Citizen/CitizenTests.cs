using FluentAssertions;
using Kmd.Momentum.Mea.Citizen;
using Kmd.Momentum.Mea.Citizen.Model;
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
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();

             var _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            var mockResponseData = new List<string>();

            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("9a8ca063-a38b-4914-bf77-9952a09d7e19", "TestDisplay1","givenname","middlename","initials","test@email.com","1234567891","","description",true,true)));
            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("21effd90-0770-4976-8416-6f1230606eea", "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));
            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}/search"))).Returns(Task.FromResult((IReadOnlyList<string>)mockResponseData));

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
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var httpClientCitizenDataResponse = JsonConvert.SerializeObject( new CitizenDataResponseModel("21effd90-0770-4976-8416-6f1230606eea", "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true));

            var _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/dummyCpr"))).Returns(Task.FromResult(httpClientCitizenDataResponse));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByCprAsync("dummyCpr").ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(JsonConvert.DeserializeObject<CitizenDataResponseModel>( httpClientCitizenDataResponse));
        }

        [Fact]
        public async Task GetCitizenByIdSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var httpClientCitizenDataResponse = JsonConvert.SerializeObject(new CitizenDataResponseModel("21effd90-0770-4976-8416-6f1230606eea", "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true));

            var _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/21effd90-0770-4976-8416-6f1230606eea"))).Returns(Task.FromResult(httpClientCitizenDataResponse));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configuration.Object);

            //Act
            var result = await citizenService.GetCitizenByIdAsync("21effd90-0770-4976-8416-6f1230606eea").ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(JsonConvert.DeserializeObject<CitizenDataResponseModel>(httpClientCitizenDataResponse));
        }
    }
}
