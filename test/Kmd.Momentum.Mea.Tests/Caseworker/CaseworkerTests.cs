using FluentAssertions;
using Kmd.Momentum.Mea.Caseworker1;
using Kmd.Momentum.Mea.Caseworker1.Model;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Tests.Caseworker
{
    public class CaseworkerTests
    {
        //[Fact]
        //public async Task GetAllCaseworkersSuccess()
        //{
        //    var helperHttpClientMoq = new Mock<ICaseworkerHttpClientHelper>();

        //    var _configuration = new Mock<IConfiguration>();
        //    _configuration.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com/");

        //    var mockResponseData = new List<string>();

        //    mockResponseData.Add(JsonConvert.SerializeObject(new CaseworkerDataResponseModel("testId1", "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));
        //    mockResponseData.Add(JsonConvert.SerializeObject(new CaseworkerDataResponseModel("testId2", "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));

        //    helperHttpClientMoq.Setup(x => x.GetAllCaseworkerDataFromMomentumCoreAsync(new Uri($"{_configuration.Object["KMD_MOMENTUM_MEA_McaApiUri"]}/punits/0d1345f4-51e0-407e-9dc0-15a9d08326d7/caseworkers")))
        //        .Returns(Task.FromResult((IReadOnlyList<CaseworkerDataResponseModel>)mockResponseData));
          

        //    var caseworkerService = new CaseworkerService(helperHttpClientMoq.Object, _configuration.Object);
        //    var responseData = mockResponseData.Select(x => JsonConvert.DeserializeObject<CaseworkerDataResponseModel>(x)).ToList();

        //    //Act
        //    var result = await caseworkerService.GetAllCaseworkersInMomentumAsync().ConfigureAwait(false);


        //    //Asert
        //    result.Should().NotBeNull();
        //    //result.IsError.Should().BeFalse();
        //    result.Should().BeEquivalentTo(mockResponseData);
        //}
    }
}
