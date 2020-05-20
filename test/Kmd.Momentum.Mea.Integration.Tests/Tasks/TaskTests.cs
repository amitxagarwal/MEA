using FluentAssertions;
using Kmd.Momentum.Mea.Caseworker.Model;
using Kmd.Momentum.Mea.TaskApi.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        [SkipLocalFact]
        public async Task UpdateTaskstatusSuccess()
        {
            //Arrange

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var dataToGetCaseworkerId = await client.GetAsync("/caseworkers?pageNumber=1").ConfigureAwait(false);
            var dataToGetCaseworkerIdBody = await dataToGetCaseworkerId.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualdataToGetCaseworkerId = JsonConvert.DeserializeObject<CaseworkerList>(dataToGetCaseworkerIdBody);
            var caseworkerId = actualdataToGetCaseworkerId.Result.Select(x => x.CaseworkerId).FirstOrDefault();

            var dataToGetTaskId = await client.GetAsync($"/caseworkers/{caseworkerId}/tasks?pageNumber=1");
            var dataToGetTaskIdBody = await dataToGetTaskId.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualdataToGetTaskId = JsonConvert.DeserializeObject<TaskList>(dataToGetTaskIdBody);
            var taskId = actualdataToGetTaskId.Result.Select(x => x.TaskId).FirstOrDefault();

            var requestUri = $"/tasks/{taskId}/update";

            var taskUpdateStatus = new TaskUpdateStatus()
            {
                TaskAction = TaskAction.Start,
                TaskContext = TaskContext.Citizens
            };
            string _serializedRequest = JsonConvert.SerializeObject(taskUpdateStatus);

            //Act
            var response = await client.PutAsync(requestUri, new StringContent(_serializedRequest, Encoding.UTF8, "application/json"));
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<TaskDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            actualResponse.TaskState.Should().BeEquivalentTo(TaskState.Started);
        }

        [SkipLocalFact]
        public async Task UpdateTaskstatusFails()
        {
            //Arrange
            var taskId = "70375a2b-14d2-4774-a9a2-ab123ebd2ff6";
            var requestUri = $"/tasks/{taskId}/updates";

            var client = _factory.CreateClient();

            var tokenHelper = new TokenGenerator();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var taskUpdateStatus = new TaskUpdateStatus()
            {
                TaskAction = TaskAction.Start,
                TaskContext = TaskContext.Citizens
            };
            string _serializedRequest = JsonConvert.SerializeObject(taskUpdateStatus);

            //Act
            var response = await client.PutAsync(requestUri, new StringContent(_serializedRequest, Encoding.UTF8, "application/json"));
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var actualResponse = JsonConvert.DeserializeObject<TaskDataResponseModel>(responseBody);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseBody.Should().BeNullOrEmpty();
            actualResponse.Should().BeNull();
        }
    }
}
