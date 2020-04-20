using FluentAssertions;
using Kmd.Momentum.Mea.Caseworker.Model;
using Newtonsoft.Json;
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
            var client = _factory.CreateClient();
            var requestUri = "/caseworkers?pageNumber=1";
            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);
            //var pageNumber = 1;

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<MeaBaseList>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.Result.Count.Should().BeGreaterThan(0);
        }

        [SkipLocalFact]
        public async Task GetCaseworkerByCaseworkerIdSuccess()
        {
            //Arrange
            var caseworkerId = "0b328744-77ef-493f-abf4-295bb824a52b";
            var requestUri = $"/caseworkers/kss/{caseworkerId}";

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CaseworkerDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.CaseworkerId.Should().BeEquivalentTo(caseworkerId);
        }
    }
}
