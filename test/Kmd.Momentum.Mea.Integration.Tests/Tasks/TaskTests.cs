using FluentAssertions;
using Kmd.Momentum.Mea.Citizen.Model;
using Kmd.Momentum.Mea.TaskApi.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Integration.Tests.Tasks
{
    public class TaskTests : IClassFixture<IntegrationTestApplicationFactory>
    {
        private readonly IntegrationTestApplicationFactory _factory;

        public TaskTests(IntegrationTestApplicationFactory factory)
        {
            _factory = factory;
        }

        //[SkipLocalFact]
        //public async Task UpdateTaskstatusSuccess()
        //{
        //    //Arrange
        //    var taskId = "926902af-b2e1-46f2-b441-a829842437b3";
        //    var taskStateValue = $"{ TaskAction.Start }";

        //    var requestUri = $"/tasks/{taskId}/{taskStateValue}?applicationContext={TaskContext.Citizens}";

        //    var client = _factory.CreateClient();

        //    var tokenHelper = new TokenGenerator();
        //    var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

        //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        //    //Act
        //    var response = await client.PutAsync(requestUri, null).ConfigureAwait(false);
        //    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        //    var actualResponse = JsonConvert.DeserializeObject<TaskDataResponseModel>(responseBody);

        //    //Assert
        //    response.StatusCode.Should().Be(HttpStatusCode.OK);
        //    actualResponse.Should().BeEquivalentTo("OK");
        //}


        [SkipLocalFact]
        public async Task UpdateTaskstatusFails()
        {
            //Arrange
            var taskId = "70375a2b-14d2-4774-a9a2-ab123ebd2ff6";
            var taskStateValue = 0;
            var requestUri = $"/task/{taskId}/{taskStateValue}?applicationContext={TaskContext.Citizens}";

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.PutAsync(requestUri, null).ConfigureAwait(false);
            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<TaskDataResponseModel>(result);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Should().BeNullOrEmpty();
            actualResponse.Should().BeNull();
        }
    }

}
