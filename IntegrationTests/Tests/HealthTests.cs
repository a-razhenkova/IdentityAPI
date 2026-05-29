using FluentAssertions;
using IntegrationTests;
using V1 = WebApi.V1;

namespace HealthTests
{
    public class HealthTests : IntegrationTestBase
    {
        public HealthTests(TestFactory factory) : base(factory) { }

        [Fact(DisplayName = "HEAD /api/v1/health/heartbeat")]
        public async Task Heartbeat()
        {
            // Act
            var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, Endpoints.Heartbeat));

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact(DisplayName = "GET /api/v1/health")]
        public async Task GetDeployInfo()
        {
            // Arrange
            var client = new HttpClientProxy(_client);
        
            // Act
            var response = await client.GetAsync<V1.DeployInfoResponse>(Endpoints.DeployInfo);

            // Assert
            response.Should().NotBeNull();
        }

        [Fact(DisplayName = "GET /api/v1/health/checks")]
        public async Task CheckHealth()
        {
            // Arrange
            var client = new HttpClientProxy(_client);

            // Act
            var response = await client.GetAsync(Endpoints.HealthChecks);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}