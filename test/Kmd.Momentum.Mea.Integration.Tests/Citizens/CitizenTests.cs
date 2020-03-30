using FluentAssertions;
using Kmd.Momentum.Mea.Citizen.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Integration.Tests.Citizens
{
    public class CitizenTests : IClassFixture<IntegrationTestApplicationFactory>
    {
        private readonly IntegrationTestApplicationFactory _factory;

        public CitizenTests(IntegrationTestApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact(Skip = "Skipping the test cases for now")]
        public async Task GetActiveCitizensSuccess()
        {
            //Arrange       
            var clientMoq = _factory.CreateClient();
            var mockResponseData = new List<string>();

            //Act
            var response = await clientMoq.GetAsync($"/citizens").ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<List<CitizenDataResponseModel>>(result);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNullOrEmpty();
            actualResponse.Count.Should().BeGreaterThan(0);
        }

        [Fact(Skip = "Skipping the test cases for now")]
        public async Task GetCitizenByCprNoSuccess()
        {
            //Arrange
            var cprNumber = "0208682105";
            var requestUri = $"/citizens/cpr/{cprNumber}";
            
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.CitizenId.Should().NotBeNullOrEmpty();
        }


        [Fact(Skip = "Skipping the test cases for now")]
        public async Task GetCitizenByCitizenIdSuccess()
        {
            //Arrange
            var citizenId = "70375a2b-14d2-4774-a9a2-ab123ebd2ff6";
            var requestUri = $"/citizens/kss/{citizenId}";
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.CitizenId.Should().BeEquivalentTo(citizenId);
        }
    }
}

