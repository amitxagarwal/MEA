using Kmd.Momentum.Mea.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Kmd.Momentum.Mea.Citizen.Model;
using Moq;
using Kmd.Momentum.Mea.Common.HttpProvider;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System;

namespace Kmd.Momentum.Mea.Integration.Tests.Citizen
{
    public class CitizenTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public CitizenTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetActiveCitizensSuccess()
        {
            //Arrange
            var uriForActiveCitizens = "/citizens";
            var httpClientMoq = new Mock<IHttpClientBuilder>();

            var mockedFactory = _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => httpClientMoq.Object);
            }));

            var client = mockedFactory.CreateClient();

            //Act
            var response = await client.GetAsync(uriForActiveCitizens).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<string[]>(result);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCitizenByCprSuccess()
        {
            //Arrange
            var cprNumber = "2811733914";
            var requestUri = $"/citizens/cpr/{cprNumber}";

            var httpClientMoq = new Mock<IHttpClientBuilder>();

            var mockedFactory = _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => httpClientMoq.Object);
            }));

            var client = mockedFactory.CreateClient();

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCitizenByCitizenIdSuccess()
        {
            //Arrange
            var citizenId = "70375a2b-14d2-4774-a9a2-ab123ebd2ff6";
            var requestUri = $"/citizens/kss/{citizenId}";

            var httpClientMoq = new Mock<IHttpClientBuilder>();

            var mockedFactory = _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => httpClientMoq.Object);
            }));

            var client = mockedFactory.CreateClient();

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
        }
    }
}
