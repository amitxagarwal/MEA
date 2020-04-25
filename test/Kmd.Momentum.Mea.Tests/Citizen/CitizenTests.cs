using FluentAssertions;
using Kmd.Momentum.Mea.Citizen;
using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.MeaHttpClientHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Tests.Citizen
{
    public class CitizenTests
    {
        private Mock<IHttpContextAccessor> GetContext()
        {
            var context = new Mock<IHttpContextAccessor>();
            var hc = new DefaultHttpContext();
            hc.TraceIdentifier = Guid.NewGuid().ToString();
            var claims = new List<Claim>()
                        {
                            new Claim("azp", Guid.NewGuid().ToString()),
                            new Claim("tenant", "159")
                        };
            var identity = new ClaimsIdentity(claims, "JWT");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            hc.User = claimsPrincipal;

            context.Setup(x => x.HttpContext).Returns(hc);
            return context;
        }
      
        [Fact]
        public async Task GetAllActiveCitizensSuccess()
        {
            //Arrange
            int pageNumber = 2;
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var context = GetContext();
            var configurationMoq = new Mock<IConfiguration>();

            var mockResponseData = new List<string>();

            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId1", "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));
            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId2", "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));

            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync("/search", pageNumber))
                .Returns(Task.FromResult(new ResultOrHttpError<IReadOnlyList<string>, Error>(mockResponseData)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);
            var responseData = mockResponseData.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x)).ToList();

            //Act
            var result = await citizenService.GetAllActiveCitizensAsync(pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(responseData);
        }

        [Fact]
        public async Task GetAllActiveCitizensFails()
        {
            //Arrange
            int pageNumber = 1;
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var context = GetContext();
            var configurationMoq = new Mock<IConfiguration>();
           
            var mockResponseData = new List<string>();

            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId1", "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));
            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId2", "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));

            var error = new Error("123456", new string[] { "An Error Occured while retriving data of all active citizens" }, "MCA");


            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync
            ("/search", pageNumber))
                .Returns(Task.FromResult(new ResultOrHttpError<IReadOnlyList<string>, Error>(error, HttpStatusCode.BadRequest)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);
            var responseData = mockResponseData.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x)).ToList();

            //Act
            var result = await citizenService.GetAllActiveCitizensAsync(pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("An Error Occured while retriving data of all active citizens");
        }

        [Fact]
        public async Task GetAllActiveCitizensFailsWhenPageNoIsLessThanOrEqualTo0()
        {
            //Arrange
            int pageNumber = -1;
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var context = GetContext();

            var configurationMoq = new Mock<IConfiguration>();

            var mockResponseData = new List<string>();

            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId1", "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));
            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId2", "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));

            var error = new Error("123456", new string[] { "PageNumber cannot be less than or equal to zero" }, "MEA");

            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync("/search", pageNumber))
                .Returns(Task.FromResult(new ResultOrHttpError<IReadOnlyList<string>, Error>(error, HttpStatusCode.BadRequest)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);
            var responseData = mockResponseData.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x)).ToList();

            //Act
            var result = await citizenService.GetAllActiveCitizensAsync(pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("PageNumber cannot be less than or equal to zero");
        }

        [Fact]
        public async Task GetAllActiveCitizensFailsWhenPageNoIsGreaterThanAvailableRecords()
        {
            //Arrange
            int pageNumber = 45;
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var context = GetContext();
            var configurationMoq = new Mock<IConfiguration>();

            var mockResponseData = new List<string>();

            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId1", "TestDisplay1", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));
            mockResponseData.Add(JsonConvert.SerializeObject(new CitizenDataResponseModel("testId2", "TestDisplay2", "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true)));

            var error = new Error("123456", new string[] { "No Records are available for entered page number" }, "MEA");

            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCoreAsync("/search", pageNumber))
                .Returns(Task.FromResult(new ResultOrHttpError<IReadOnlyList<string>, Error>(error, HttpStatusCode.BadRequest)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);
            var responseData = mockResponseData.Select(x => JsonConvert.DeserializeObject<CitizenDataResponseModel>(x)).ToList();

            //Act
            var result = await citizenService.GetAllActiveCitizensAsync(pageNumber).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("No Records are available for entered page number");
        }

        [Fact]
        public async Task GetCitizenByCprSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var context = GetContext();
            var cpr = "1234567890";
            var configurationMoq = new Mock<IConfiguration>();
         
            var citizenData = new CitizenDataResponseModel("testId1", "TestDisplay1",
                "givenname", "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true);

            var httpClientCitizenDataResponse = JsonConvert.SerializeObject(citizenData);


            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync($"citizens/{cpr}"))
                .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(httpClientCitizenDataResponse)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await citizenService.GetCitizenByCprAsync(cpr).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(citizenData);
        }

        [Fact]
        public async Task GetCitizenDataByCprFails()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var context = GetContext();
            var cpr = "1234567890";
            var configurationMoq = new Mock<IConfiguration>();

            var citizenDataResponse = new CitizenDataResponseModel("test-test-test-test-test", "test display name",
                "", "", "", "test@test.com", "+99999999", "", "");


            var error = new Error("123456", new string[] { "Citizen with the supplied cpr no is not found" }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync($"citizens/{cpr}"))
                .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(error, HttpStatusCode.BadRequest)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await citizenService.GetCitizenByCprAsync(cpr).ConfigureAwait(false);

            //Asert
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("Citizen with the supplied cpr no is not found");
        }

        [Fact]
        public async Task GetCitizenByCitizenIdSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var context = GetContext();
            var citizenId = "1234567890";
            var configurationMoq = new Mock<IConfiguration>();

            var citizenData = new CitizenDataResponseModel(citizenId, "TestDisplay1", "givenname",
                "middlename", "initials", "test@email.com", "1234567891", "", "description", true, true);
            var httpClientCitizenDataResponse = JsonConvert.SerializeObject(citizenData);

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync($"citizens/{citizenId}"))
                .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(httpClientCitizenDataResponse)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await citizenService.GetCitizenByIdAsync(citizenId).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo(citizenData);
        }


        [Fact]
        public async Task GetCitizenByCitizenIdFails()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var context = GetContext();
            var citizenId = "1234567890";
            var configurationMoq = new Mock<IConfiguration>();

            var citizenDataResponse = new CitizenDataResponseModel(citizenId, "test display name",
                "", "", "", "test@test.com", "+99999999", "", "");

            var error = new Error("123456", new string[] { "Citizen with the supplied cpr no is not found" }, "MCA");

            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCoreAsync($"citizens/{citizenId}"))
                .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(error, HttpStatusCode.BadRequest)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await citizenService.GetCitizenByIdAsync(citizenId).ConfigureAwait(false);

            //Asert
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().Be("Citizen with the supplied cpr no is not found");
        }

        [Fact]
        public async Task CreateJournalNoteAsyncSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var context = GetContext();
            var configurationMoq = new Mock<IConfiguration>();

            JournalNoteDocumentResponseModel[] requestDocumentModel = { new JournalNoteDocumentResponseModel() {
                Content="testContent",
                ContentType="testContentType",
                Name="testDocumentName"
            } };

            var requestModel = new JournalNoteResponseModel()
            {
                Cpr = "testCpr",
                Body = "testBody",
                Title = "testTitle",
                Type = "testType",
                Documents = requestDocumentModel
            };

            helperHttpClientMoq.Setup(x => x.CreateJournalNoteInMomentumCoreAsync("journals/note", "testCitizenId", requestModel))
                .Returns(Task.FromResult(new ResultOrHttpError<string, Error>("")));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await citizenService.CreateJournalNoteAsync("testCitizenId", requestModel).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.IsError.Should().BeFalse();
            result.Result.Should().BeEquivalentTo("");
        }

        [Fact]
        public async Task CreateJournalNoteAsyncFail()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<ICitizenHttpClientHelper>();
            var context = GetContext();
            var configurationMoq = new Mock<IConfiguration>();

            JournalNoteDocumentResponseModel[] requestDocumentModel = { new JournalNoteDocumentResponseModel() {
                Content="testContent",
                ContentType="testContentType",
                Name="testDocumentName"
            } };

            var requestModel = new JournalNoteResponseModel()
            {
                Cpr = "testCpr",
                Body = "testBody",
                Title = "testTitle",
                Type = "testType",
                Documents = requestDocumentModel
            };

            var error = new Error("123456", new string[] { "Some error occured when creating note" }, "MCA");

            helperHttpClientMoq.Setup(x => x.CreateJournalNoteInMomentumCoreAsync("journals/note", "testCitizenId", requestModel))
                .Returns(Task.FromResult(new ResultOrHttpError<string, Error>(error, HttpStatusCode.BadRequest)));

            var citizenService = new CitizenService(helperHttpClientMoq.Object, configurationMoq.Object, context.Object);

            //Act
            var result = await citizenService.CreateJournalNoteAsync("testCitizenId", requestModel).ConfigureAwait(false);

            //Asert            
            result.IsError.Should().BeTrue();
            result.Error.Errors[0].Should().BeEquivalentTo("Some error occured when creating note");
        }
    }
}
