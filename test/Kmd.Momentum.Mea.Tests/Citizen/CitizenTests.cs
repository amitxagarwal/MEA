using FluentAssertions;
using Kmd.Momentum.Mea.Api;
using Kmd.Momentum.Mea.Api.Citizen;
using Kmd.Momentum.Mea.Api.Common;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Tests.Citizen
{
    public class CitizenTests
    {

        [Fact]
        public async Task GetAllActiveCitizensSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHelperHttpClient>();
            var cprArray = new string[] { "1234", "12345" };
            var _configurationRoot = new Mock<IConfiguration>();
            _configurationRoot.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com");

#pragma warning disable CA2000 // Dispose objects before losing scope
            helperHttpClientMoq.Setup(x => x.GetAllActiveCitizenDataFromMomentumCore(new Uri($"{_configurationRoot.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/withActiveClassification"))).Returns(Task.FromResult( cprArray));
#pragma warning restore CA2000 // Dispose objects before losing scope

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configurationRoot.Object);

            //Act
            var result = await citizenService.GetAllActiveCitizens().ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(cprArray);
        }

        [Fact]
        public async Task GetCitizenByCprSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHelperHttpClient>();
            var primaryAddress = new PrimaryAddress("building", "city", "coName", "end", "floor", "id", true, true, true, "latitude", "longitutde", "postalCode",
                "start", "street", "suite", null);
            
            var responsibleActors = new ResponsibleActors() { ActorId = "actor", ResponsibilityCodes = new int?[] { 1, 2 }, Role = 1 };
            var responsibleActorsArray = new ResponsibleActors[] { responsibleActors };
            var enrollmentStatus = new CitizenEnrollmentStatus() { EndDate = DateTimeOffset.UtcNow, Status = 1 };
            var profile = new CitizenProfile()
            {
                GuestAuthorityParticipationUnwantedByCitizen = true,
                GuestAuthorityParticipationUnwantedByGuestAuthority = true,
                ParticipationUnwantedByCitizenUpdatedAt = "ParticipationUnwantedByCitizenUpdatedAt",
                ParticipationUnwantedByGuestAuthorityUpdatedAt = "ParticipationUnwantedByGuestAuthorityUpdatedAt"
            };

            var contactInformation = new ContactInformation() { 
             Email = "test@test.com",
             Phone = "11111111",
             PrimaryAddress= primaryAddress,
             TemporaryAddress ="temporary address"};

            var citizenDataModel = new CitizenDataModel("cpr",37,1,1,1,true, responsibleActorsArray, DateTimeOffset.UtcNow,contactInformation,true,1,"targetgroupcode",
                true,"jobcentrename", profile, "isDead", "married","citizenshipcode", enrollmentStatus, "currentcontact groupcode","currentpersponcategory",
                "ishandled","civilregistrationstatus","1","displayname");

            var citizenDataResponse =
                new CitizenDataResponse(citizenDataModel.Id, null, citizenDataModel.DisplayName, null, null, null, citizenDataModel.ContactInformation.Email,
                citizenDataModel.ContactInformation.Phone, null, null, null);

            var _configurationRoot = new Mock<IConfiguration>();
            _configurationRoot.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com");

#pragma warning disable CA2000 // Dispose objects before losing scope
            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCore(new Uri($"{_configurationRoot.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenDataModel.Cpr}"))).Returns(Task.FromResult(citizenDataModel));
#pragma warning restore CA2000 // Dispose objects before losing scope

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configurationRoot.Object);

            //Act
            var result = await citizenService.GetCitizenByCpr(citizenDataModel.Cpr).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(citizenDataResponse);
        }

        [Fact]
        public async Task GetCitizenByIdSuccess()
        {
            //Arrange
            var helperHttpClientMoq = new Mock<IHelperHttpClient>();
            var primaryAddress = new PrimaryAddress("building", "city", "coName", "end", "floor", "id", true, true, true, "latitude", "longitutde", "postalCode",
                "start", "street", "suite", null);

            var responsibleActors = new ResponsibleActors() { ActorId = "actor", ResponsibilityCodes = new int?[] { 1, 2 }, Role = 1 };
            var responsibleActorsArray = new ResponsibleActors[] { responsibleActors };
            var enrollmentStatus = new CitizenEnrollmentStatus() { EndDate = DateTimeOffset.UtcNow, Status = 1 };
            var profile = new CitizenProfile()
            {
                GuestAuthorityParticipationUnwantedByCitizen = true,
                GuestAuthorityParticipationUnwantedByGuestAuthority = true,
                ParticipationUnwantedByCitizenUpdatedAt = "ParticipationUnwantedByCitizenUpdatedAt",
                ParticipationUnwantedByGuestAuthorityUpdatedAt = "ParticipationUnwantedByGuestAuthorityUpdatedAt"
            };

            var contactInformation = new ContactInformation()
            {
                Email = "test@test.com",
                Phone = "11111111",
                PrimaryAddress = primaryAddress,
                TemporaryAddress = "temporary address"
            };

            var citizenDataModel = new CitizenDataModel("cpr", 37, 1, 1, 1, true, responsibleActorsArray, DateTimeOffset.UtcNow, contactInformation, true, 1, "targetgroupcode",
                true, "jobcentrename", profile, "isDead", "married", "citizenshipcode", enrollmentStatus, "currentcontact groupcode", "currentpersponcategory",
                "ishandled", "civilregistrationstatus", "1", "displayname");

            var citizenDataResponse =
               new CitizenDataResponse(citizenDataModel.Id, null, citizenDataModel.DisplayName, null, null, null, citizenDataModel.ContactInformation.Email,
               citizenDataModel.ContactInformation.Phone, null, null, null);


            var _configurationRoot = new Mock<IConfiguration>();
            _configurationRoot.SetupGet(x => x["KMD_MOMENTUM_MEA_McaApiUri"]).Returns("http://google.com");

#pragma warning disable CA2000 // Dispose objects before losing scope
            helperHttpClientMoq.Setup(x => x.GetCitizenDataByCprOrCitizenIdFromMomentumCore(new Uri($"{_configurationRoot.Object["KMD_MOMENTUM_MEA_McaApiUri"]}citizens/{citizenDataModel.Id}"))).Returns(Task.FromResult(citizenDataModel));
#pragma warning restore CA2000 // Dispose objects before losing scope

            var citizenService = new CitizenService(helperHttpClientMoq.Object, _configurationRoot.Object);

            //Act
            var result = await citizenService.GetCitizenById(citizenDataModel.Id).ConfigureAwait(false);

            //Asert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(citizenDataResponse);
        }
    }
}
