using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Tests.Caseworker
{
    public class CaseworkerTests
    {
        [Fact]
        public async Task GetAllCaseworkersSuccess()
        {
            var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();

            var _configuration = new Mock<IConfiguration>();
            _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

            var mockResponseData = new List<string>();

            mockResponseData.Add(JsonConvert.SerializeObject(new CaseworkerDataResponseModel("testId1", "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));
            mockResponseData.Add(JsonConvert.SerializeObject(new CaseworkerDataResponseModel("testId2", "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));

            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}/search")))
                .Returns(Task.FromResult(new IReadOnlyList<string>(mockResponseData)));

            var citizenService = new CaseworkerService(helperHttpClientMoq.Object, _configuration.Object);
            var responseData = mockResponseData.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x)).ToList();

            //Act
            var result = await citizenService.GetAllActiveCitizensAsync().ConfigureAwait(false);


            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(responseData);
        }
    }
}
