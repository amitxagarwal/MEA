using FluentAssertions;
using Kmd.Momentum.Mea.Citizen.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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

        [SkipLocalFact]
        public async Task GetActiveCitizensSuccess()
        {
            //Arrange       
            var clientMoq = _factory.CreateClient();
            var pageNumber = 2;
            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            clientMoq.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await clientMoq.GetAsync($"/citizens?pagenumber={pageNumber}").ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenList>(result);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Result.Should().NotBeNullOrEmpty();
            actualResponse.TotalNoOfPages.Should().NotBe(0);
        }

        [SkipLocalFact]
        public async Task GetActiveCitizensFails()
        {
            //Arrange       
            var clientMoq = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            clientMoq.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await clientMoq.GetAsync($"/citizen").ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [SkipLocalFact]
        public async Task GetCitizenByCprNoSuccess()
        {
            //Arrange
            var cprNumber = "0208682105";//we are hard coding the cpr number because we dont have any api in MEA which is returning cpr as response.
            var requestUri = $"/citizens/cpr/{cprNumber}";

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.CitizenId.Should().NotBeNullOrEmpty();
        }

        [SkipLocalFact]
        public async Task GetCitizenByCprNoFails()
        {
            //Arrange
            var cprNumber = "1234567890";
            var requestUri = $"/citizens/cpr/{cprNumber}";

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var error = "[\"1014 - Oplysningerne kunne ikke valideres hos DFDG. Prøv igen eller meld fejlen hos Momentum-support.\"]";

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should().BeEquivalentTo(error);
        }

        [SkipLocalFact]
        public async Task GetCitizenByCitizenIdSuccess()
        {
            //Arrange
            var pageNumber = 1;

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var dataToGetCitizenId = await client.GetAsync($"/citizens?pagenumber={pageNumber}").ConfigureAwait(false);
            var dataBody = await dataToGetCitizenId.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualData = JsonConvert.DeserializeObject<CitizenList>(dataBody);
            var citizenId = actualData.Result.Select(x => x.CitizenId).FirstOrDefault();

            var requestUri = $"/citizens/kss/{citizenId}";


            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<CitizenDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().NotBeNull();
            actualResponse.CitizenId.Should().BeEquivalentTo(citizenId);
        }

        [SkipLocalFact]
        public async Task GetCitizenByCitizenIdFails()
        {
            //Arrange
            var citizenId = "836e2ad4-d028-4e3d-bb01-fdd60bca9b81";
            var requestUri = $"/citizens/kss/{citizenId}";

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
             var error = "[\"An error occured while fetching the record(s) from Core Api\"]";

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should().BeEquivalentTo(error);
        }

        [SkipLocalFact]
        public async Task CreateJournalNoteAsyncSuccess()
        {
            //Arrange
            var pageNumber = 1;
            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var dataToGetCitizenId = await client.GetAsync($"/citizens?pagenumber={pageNumber}").ConfigureAwait(false);
            var dataBody = await dataToGetCitizenId.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualData = JsonConvert.DeserializeObject<CitizenList>(dataBody);
            var momentumCitizenId = actualData.Result.Select(x => x.CitizenId).FirstOrDefault();

            var requestUri = $"/citizens/journal/{momentumCitizenId}";

            List<JournalNoteDocumentRequestModel> documentList = new List<JournalNoteDocumentRequestModel>()
            {
                new JournalNoteDocumentRequestModel()
                {
                    Content = "testContent",
                    ContentType = "application/octet-stream",
                    Name = "TestName.pdf"
                }
            };

            JournalNoteRequestModel mcaRequestModel = new JournalNoteRequestModel()
            {
                Cpr = "0101005402",
                Title = "testTitle",
                Body = "testBody",
                Type = JournalNoteType.SMS,
                Documents = documentList
            };
            string _serializedRequest = JsonConvert.SerializeObject(mcaRequestModel);

            //Act
            var response = await client.PostAsync(requestUri, new StringContent(_serializedRequest, Encoding.UTF8, "application/json"));
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.Should().BeEquivalentTo("OK");
        }
    }
}