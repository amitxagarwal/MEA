using FluentAssertions;
using Kmd.Momentum.Mea.Caseworker.Model;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Headers;
using Kmd.Momentum.Mea.TaskApi.Model;
using System.Linq;

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

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CaseworkerList>(responseBody);
            actualResponse.Should().NotBeNull();
            actualResponse.Result.Count.Should().BeGreaterThan(0);
        }

        [SkipLocalFact]
        public async Task GetAllCaseworkersFails()
        {
            //Arrange       
            var client = _factory.CreateClient();
            var requestUri = "/caseworker?pageNumber=1";
            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);            

            //Assert
            response.StatusCode.Should().Be((HttpStatusCode.NotFound));
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CaseworkerList>(responseBody);
            responseBody.Should().BeNullOrEmpty();
            actualResponse.Should().BeNull();
        }

        [SkipLocalFact]
        public async Task GetCaseworkerByCaseworkerIdSuccess()
        {
            //Arrange
            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var dataToGetCaseworkerId = await client.GetAsync("/caseworkers?pageNumber=1").ConfigureAwait(false);
            var dataBody = await dataToGetCaseworkerId.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualData = JsonConvert.DeserializeObject<CaseworkerList>(dataBody);
            var caseworkerId = actualData.Result.Select(x => x.CaseworkerId).FirstOrDefault();

            var requestUri = $"/caseworkers/kss/{caseworkerId}";

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CaseworkerDataResponseModel>(responseBody);
            actualResponse.Should().NotBeNull();
            actualResponse.CaseworkerId.Should().Be(caseworkerId);
        }

        [SkipLocalFact]
        public async Task GetCaseworkerByCaseworkerIdFails()
        {
            //Arrange
            var caseworkerId = "70375a2b-14d2-4774-a9a2-ab123ebd2ff6";
            var requestUri = $"/caseworkers/kss/{caseworkerId}";

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);            

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var error = "[\"An error occured while fetching the record(s) from Core Api\"]";
            result.Should().BeEquivalentTo(error);
        }

        [SkipLocalFact]
        public async Task GetCaseworkerByCaseworkerIdFailsDueToInvalidReq()
        {
            //Arrange
            var caseworkerId = "0b328744-77ef-493f-abf4-295bb824a52bdfvsdgvdrgbd";
            var requestUri = $"/caseworkers/kss/{caseworkerId}";

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var error = "[\"{\\\"message\\\":\\\"The request is invalid.\\\"}\"]";
            result.Should().BeEquivalentTo(error);
        }

        [SkipLocalFact]
        public async Task GetAllTasksByCaseworkerIdSuccess()
        {
            //Arrange       
            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var dataToGetCaseworkerId = await client.GetAsync("/caseworkers?pageNumber=1").ConfigureAwait(false);
            var dataBody = await dataToGetCaseworkerId.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualData = JsonConvert.DeserializeObject<CaseworkerList>(dataBody);
            var caseworkerId = actualData.Result.Select(x => x.CaseworkerId).FirstOrDefault();

            var requestUri = $"/caseworkers/{caseworkerId}/tasks?pageNumber=1";

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<TaskList>(responseBody);
            actualResponse.Should().NotBeNull();
            actualResponse.Result.Count.Should().BeGreaterThan(0);
        }

        [SkipLocalFact]
        public async Task GetAllTasksByCaseworkerIdFails()
        {
            //Arrange       
            var client = _factory.CreateClient();
            var caseworkerId = "70375a2b-14d2-4774-a9a2-ab123ebd2ff6";
            var requestUri = $"/caseworker/{caseworkerId}/tasks?pageNumber=1";
            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<TaskList>(responseBody);
            responseBody.Should().BeNullOrEmpty();
            actualResponse.Should().BeNull();
        }
    }

}

