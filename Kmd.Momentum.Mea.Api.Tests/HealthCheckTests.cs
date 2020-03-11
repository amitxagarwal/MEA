using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmd.Momentum.Mea.Test.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Kmd.Momentum.Mea.Api.Tests
{
    public class HealthCheckTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public HealthCheckTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task TheHealthCheckStatusResponseIsHealthy()
        {
            //Arrange
            var client = _factory.CreateClient();
            var tokenHelper = new TokenHelper();
            var accessToken = await tokenHelper.GetToken().ConfigureAwait(false);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            //Act
            var response = await client.GetAsync(new Uri("Health/Ready", UriKind.Relative)).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);           
        }
    }
}
