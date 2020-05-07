using FluentAssertions;
using Kmd.Momentum.Mea.Caseworker.Model;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http.Headers;

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
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CaseworkerList>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
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
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CaseworkerList>(responseBody);

            //Assert
            response.StatusCode.Should().Be((HttpStatusCode.NotFound));
            responseBody.Should().BeNullOrEmpty();
            actualResponse.Should().BeNull();
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

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CaseworkerDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.CaseworkerId.Should().BeEquivalentTo(caseworkerId);
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
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var error = "[\"An error occured while fetching the record(s) from Core Api\"]";

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should().BeEquivalentTo(error);
        }

        [Fact]
        public async Task GetCaseworkerByCaseworkerIdFailsDueToInvalidReq()
        {
            //Arrange
            var caseworkerId = "0b328744-77ef-493f-abf4-295bb824a52bdfvsdgvdrgbd";
            var requestUri = $"/caseworkers/kss/{caseworkerId}";

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6InpGTDB5ZGF2NDgxSHJDT2M0bk9MdTR2cDB2SDZxeTBvUS1SdlBzQW1vNlkifQ.eyJpc3MiOiJodHRwczovL2xvZ2ljaWRlbnRpdHlwcm9kLmIyY2xvZ2luLmNvbS9hYTBkNmYzYi04ZmIyLTQ3N2QtYWI4OC1iYjJlNjY0NjIwZTgvdjIuMC8iLCJleHAiOjE1ODg4MzU4NTksIm5iZiI6MTU4ODgzMjI1OSwiYXVkIjoiNjlkOTY5M2UtYzRiNy00Mjk0LWEyOWYtY2RkYWViZmE1MThiIiwic3ViIjoiMWQxOGQxNTEtNTE5Mi00N2YxLWE2MTEtZWZhNTBkYmRjNDMxIiwiaWRwIjoiTG9naWNJZGVudGl0eSIsImF1dGhlbnRpY2F0aW9uU291cmNlIjoiQ2xpZW50IENyZWRlbnRpYWxzIiwidGVuYW50IjoiMTU5Iiwibm9uY2UiOiJkZWZhdWx0Tm9uY2UiLCJzY3AiOiJhY2Nlc3MiLCJhenAiOiIxZDE4ZDE1MS01MTkyLTQ3ZjEtYTYxMS1lZmE1MGRiZGM0MzEiLCJ2ZXIiOiIxLjAiLCJpYXQiOjE1ODg4MzIyNTl9.LfxDczrKP15Lh8oLc7pnJneyJnxq2pgqxbqIqZagOiHFkkaFcslcp4gSHUDPIjHlINTvsWG8uYkIMubDkpSD8qmVXX7T162pGHzjhixeiqE3cQVbpaiDdPwD7WfC4H8nTwgQFiJ6NYIQJoARQObduTzNkkiURaViDy4-cgPp-efQjEiOgDRRoPBzEvy8j1WGknAtrdW0QjD137TRH77uh7FeOBPK_R6RXS_Ln_ZoeSqZ7NQC0MsINTDBi1WrTc-YlKXQ6Oz3FEJFb2CGkUaBuWhpQTF9g7MdLZL7SFwrRLxiKIfy1RGN6lW-0rZA5lP65j_SiEd_c47hiy7v2HoQHw");

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var error = "[\"{\\\"message\\\":\\\"The request is invalid.\\\"}\"]";

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should().BeEquivalentTo(error);
        }
    }

}

