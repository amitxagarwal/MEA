using FluentAssertions;
using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.Caseworker1.Model;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Integration.Tests.Caseworker
{
    public class CaseworkerTests : IClassFixture<IntegrationTestApplicationFactory>
    {
        private readonly IntegrationTestApplicationFactory _factory;

        public CaseworkerTests(IntegrationTestApplicationFactory factory)
        {
            _factory = factory;
        }

        [SkipLocalFact]
        public async Task GetAllCaseworkersSuccess()
        {
            //Arrange       
            var clientMoq = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            clientMoq.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await clientMoq.GetAsync($"/caseworkers").ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<List<CaseworkerDataResponseModel>>(result);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNullOrEmpty();
            actualResponse.Count.Should().BeGreaterThan(0);
        }
    }
}
