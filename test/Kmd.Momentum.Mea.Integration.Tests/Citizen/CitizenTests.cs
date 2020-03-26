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
using Microsoft.Extensions.Configuration;

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
            var listOfCpr = new string[] { "12345", "67890" };
            var httpClientHelperMoq = new Mock<IHttpClientHelper>();

            var mockedFactory = _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => httpClientHelperMoq.Object);
            }));

            var clientMoq = mockedFactory.CreateClient();

            httpClientHelperMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri("https://kmd-rct-momentum-159-api.azurewebsites.net/api/citizens/withActiveClassification"))).Returns(Task.FromResult(listOfCpr));            

            //Act
            var response = await clientMoq.GetAsync($"/citizens").ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<string[]>(result);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNullOrEmpty();
            actualResponse.Should().BeEquivalentTo(listOfCpr);
        }
        
        [Fact]
        public async Task GetCitizenByCprNoSuccess()
        {
            //Arrange
            var cprNumber = "0208682105";
            var requestUri = $"/citizens/cpr/{cprNumber}";
            var httpClientHelperMoq = new Mock<IHttpClientHelper>();
            var httpClientCitizenDataResponse = "{\"cpr\":\"0208682105\",\"id\":\"test-test-test-test-test\",\"displayName\":\"test display name\",\"" +
               "contactInformation\":{\"email\":{\"id\":\"testId-testId-testId-testId-testId\",\"address\":\"test@test.com\"},\"phone\":{\"id\":\"testId-testId-testId-testId-testId\",\"number\":\"+99999999\",\"isMobile\":true}}}";

            var citizenDataResponse = new CitizenDataResponseModel("test-test-test-test-test", "test display name", "", "", "", "test@test.com", "+99999999", "", "");

            var mockedFactory = _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => httpClientHelperMoq.Object);
            }));
            var client = mockedFactory.CreateClient();

            httpClientHelperMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri("https://kmd-rct-momentum-159-api.azurewebsites.net/api/citizens/0208682105"))).Returns(Task.FromResult(httpClientCitizenDataResponse));


            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.Should().BeEquivalentTo(citizenDataResponse);
        }


        [Fact]
        public async Task GetCitizenByCitizenIdSuccess()
        {
            //Arrange
            var citizenId = "70375a2b-14d2-4774-a9a2-ab123ebd2ff6";
            var requestUri = $"/citizens/kss/{citizenId}";

            var httpClientHelperMoq = new Mock<IHttpClientHelper>();
            var httpClientCitizenDataResponse = "{\"cpr\":\"0208682105\",\"id\":\"70375a2b-14d2-4774-a9a2-ab123ebd2ff6\",\"displayName\":\"test display name\",\"" +
               "contactInformation\":{\"email\":{\"id\":\"testId-testId-testId-testId-testId\",\"address\":\"test@test.com\"},\"phone\":{\"id\":\"testId-testId-testId-testId-testId\",\"number\":\"+99999999\",\"isMobile\":true}}}";

            var citizenDataResponse = new CitizenDataResponseModel("70375a2b-14d2-4774-a9a2-ab123ebd2ff6", "test display name", "", "", "", "test@test.com", "+99999999", "", "");

            var mockedFactory = _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => httpClientHelperMoq.Object);
            }));

            var client = mockedFactory.CreateClient();

            httpClientHelperMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync(new Uri("https://kmd-rct-momentum-159-api.azurewebsites.net/api/citizens/70375a2b-14d2-4774-a9a2-ab123ebd2ff6"))).Returns(Task.FromResult(httpClientCitizenDataResponse));

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.Should().BeEquivalentTo(citizenDataResponse);
        }
    }
}
