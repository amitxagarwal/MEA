using FluentAssertions;
using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
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
            var citizenId = new Guid();
            var httpClientHelperMoq = new Mock<CitizenHttpClientHelper>();
            var meaHttpClientMoq = new Mock<MeaClient>();

            var mockedFactory = _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.AddScoped(_ => httpClientHelperMoq.Object);
                services.AddScoped(_ => meaHttpClientMoq.Object);
            }));

            var clientMoq = mockedFactory.CreateClient();

            var mockResponseData = new List<string>();

            List<CitizenDataResponseModel> lst = new List<CitizenDataResponseModel>()
            {
                new CitizenDataResponseModel(citizenId.ToString(), "TestDisplay1", "givenname", "middlename", "initials",
                "test@email.com", "1234567891", "", "description"),
                new CitizenDataResponseModel(citizenId.ToString(), "TestDisplay2", "givenname", "middlename", "initials",
                "test@email.com", "1234567891", "", "description")
            };

            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel(citizenId.ToString(), "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));
            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel(citizenId.ToString(), "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));

            httpClientHelperMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync(new Uri("https://kmd-rct-momentum-159-api.azurewebsites.net/api//search")))
                .Returns(Task.FromResult((IReadOnlyList<string>)mockResponseData));

            //Act
            var response = await clientMoq.GetAsync($"/citizens").ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<List<CitizenDataResponseModel>>(result);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNullOrEmpty();
            actualResponse.Should().BeEquivalentTo(lst);
        }

        [Fact(Skip = "Skipping the test cases for now")]
        public async Task GetCitizenByCprNoSuccess()
        {
            //Arrange
            var citizenId = new Guid();
            var cprNumber = "0208682105";
            var requestUri = $"/citizens/cpr/{cprNumber}";
            var httpClientHelperMoq = new Mock<CitizenHttpClientHelper>();

            var citizenDataResponse = new CitizenDataResponseModel(citizenId.ToString(), "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description");
            var httpClientCitizenDataResponse = JsonConvert.SerializeObject(citizenDataResponse);

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


        [Fact(Skip = "Skipping the test cases for now")]
        public async Task GetCitizenByCitizenIdSuccess()
        {
            //Arrange
            var citizenId = "70375a2b-14d2-4774-a9a2-ab123ebd2ff6";
            var requestUri = $"/citizens/kss/{citizenId}";

            var httpClientHelperMoq = new Mock<CitizenHttpClientHelper>();

            var citizenDataResponse = new CitizenDataResponseModel(citizenId.ToString(), "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description");
            var httpClientCitizenDataResponse = JsonConvert.SerializeObject(citizenDataResponse);

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

