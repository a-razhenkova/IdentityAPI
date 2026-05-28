using FluentAssertions;
using IntegrationTests;
using V1 = WebApi.V1;

namespace HealthTests
{
    public class HealthTests : IntegrationTestBase
    {
        [Fact(DisplayName = "HEAD /api/v1/health/heartbeat")]
        public async Task HeartbeatAsync()
        {
            // Arrange
            var httpClient = CreateClient();

            // Act
            var httpResponse = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/api/v1/health/heartbeat"));

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact(DisplayName = "GET /api/v1/health")]
        public async Task GetDeployInfo()
        {
            // Arrange
            var httpClient = new HttpClientProxy(CreateClient());
        
            // Act
            var response = await httpClient.GetAsync<V1.DeployInfoResponse>("api/v1/health");

            // Assert
            response.Should().NotBeNull();
        }

        [Fact(DisplayName = "GET /api/v1/health/checks")]
        public async Task CheckHealthAsync()
        {
            // Arrange
            var httpClient = new HttpClientProxy(CreateClient());

            // Act
            var httpResponse = await httpClient.GetAsync("api/v1/health/checks");

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}