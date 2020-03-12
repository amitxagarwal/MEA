using Kmd.Momentum.Mea.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Kmd.Momentum.Mea.Integrations.Tests
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

            //Act
            var response = await client.GetAsync(new Uri("Health/Ready", UriKind.Relative)).ConfigureAwait(false);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
