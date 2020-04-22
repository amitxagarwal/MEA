﻿//using FluentAssertions;
//using Kmd.Momentum.Mea.Citizen.Model;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace Kmd.Momentum.Mea.Integration.Tests.Citizens
//{
//    public class CitizenTests : IClassFixture<IntegrationTestApplicationFactory>
//    {
//        private readonly IntegrationTestApplicationFactory _factory;

//        public CitizenTests(IntegrationTestApplicationFactory factory)
//        {
//            _factory = factory;
//        }

//        [SkipLocalFact(Skip = "")]
//        public async Task GetActiveCitizensSuccess()
//        {
//            //Arrange       
//            var clientMoq = _factory.CreateClient();

//            var tokenHelper = new TokenGenerator();
//            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

//            clientMoq.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

//            //Act
//            var response = await clientMoq.GetAsync($"/citizens").ConfigureAwait(false);
//            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//            var actualResponse = JsonConvert.DeserializeObject<List<CitizenDataResponseModel>>(result);

//            //Assert
//            response.StatusCode.Should().Be(HttpStatusCode.OK);
//            actualResponse.Should().NotBeNullOrEmpty();
//            actualResponse.Count.Should().BeGreaterThan(0);
//        }

//        [SkipLocalFact(Skip = "")]
//        public async Task GetActiveCitizensFails()
//        {
//            //Arrange       
//            var clientMoq = _factory.CreateClient();

//            var tokenHelper = new TokenGenerator();
//            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

//            clientMoq.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

//            //Act
//            var response = await clientMoq.GetAsync($"/citizen").ConfigureAwait(false);
//            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//            var actualResponse = JsonConvert.DeserializeObject<List<CitizenDataResponseModel>>(result);

//            //Assert
//            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
//            result.Should().BeNullOrEmpty();
//            actualResponse.Should().BeNullOrEmpty();
//        }

//        [SkipLocalFact(Skip = "")]
//        public async Task GetCitizenByCprNoSuccess()
//        {
//            //Arrange
//            var cprNumber = "0208682105";
//            var requestUri = $"/citizens/cpr/{cprNumber}";

//            var client = _factory.CreateClient();

//            var tokenHelper = new TokenGenerator();
//            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

//            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

//            //Act
//            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
//            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//            var actualResponse = JsonConvert.DeserializeObject<CitizenDataResponseModel>(responseBody);

//            //Assert
//            response.StatusCode.Should().Be(HttpStatusCode.OK);
//            actualResponse.Should().NotBeNull();
//            actualResponse.CitizenId.Should().NotBeNullOrEmpty();
//        }

//        [SkipLocalFact(Skip = "")]
//        public async Task GetCitizenByCprNoFails()
//        {
//            //Arrange
//            var cprNumber = "1234567890";
//            var requestUri = $"/citizens/cpr/{cprNumber}";

//            var client = _factory.CreateClient();

//            var tokenHelper = new TokenGenerator();
//            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

//            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

//            //Act
//            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
//            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//            var error = "[\"An error occured while fetching the record(s) from Core Api\"]";

//            //Assert
//            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//            result.Should().BeEquivalentTo(error);
//        }

//        [SkipLocalFact(Skip = "")]
//        public async Task GetCitizenByCitizenIdSuccess()
//        {
//            //Arrange
//            var citizenId = "70375a2b-14d2-4774-a9a2-ab123ebd2ff6";
//            var requestUri = $"/citizens/kss/{citizenId}";

//            var client = _factory.CreateClient();

//            var tokenHelper = new TokenGenerator();
//            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

//            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

//            //Act
//            var response = await client.GetAsync(requestUri).ConfigureAwait(false);
//            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//            var actualResponse = JsonConvert.DeserializeObject<CitizenDataResponseModel>(responseBody);

//            //Assert
//            response.StatusCode.Should().Be(HttpStatusCode.OK);
//            actualResponse.Should().NotBeNull();
//            actualResponse.CitizenId.Should().BeEquivalentTo(citizenId);
//        }

//        [SkipLocalFact(Skip = "")]
//        public async Task CreateJournalNoteAsyncSuccess()
//        {
//            //Arrange
//            var momentumCitizenId = "65cd9dba-96db-40b0-9f3b-d773881afc61";
//            var requestUri = $"/citizens/journal/{momentumCitizenId}";

//            var client = _factory.CreateClient();

//            var tokenHelper = new TokenGenerator();
//            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

//            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

//            List<JournalNoteDocumentResponseModel> documentList = new List<JournalNoteDocumentResponseModel>()
//            {
//                new JournalNoteDocumentResponseModel()
//                {
//                    Content = "testContent",
//                    ContentType = "application/octet-stream",
//                    Name = "TestName.pdf"
//                }
//            };

//            JournalNoteResponseModel mcaRequestModel = new JournalNoteResponseModel()
//            {
//                Cpr = "0101005402",
//                Title = "testTitle",
//                Body = "testBody",
//                Type = "SMS",
//                Documents = documentList
//            };
//            string _serializedRequest = JsonConvert.SerializeObject(mcaRequestModel);

//            //Act
//            var response = await client.PostAsync(requestUri, new StringContent(_serializedRequest, Encoding.UTF8, "application/json"));
//            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//            var actualResponse = JsonConvert.DeserializeObject(responseBody);

//            //Assert
//            response.StatusCode.Should().Be(HttpStatusCode.OK);
//            actualResponse.Should().BeEquivalentTo("OK");
//        }
//    }
//}