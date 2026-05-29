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
            // Arrange
            var httpClient = CreateClient();

            // Act
            var httpResponse = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, Endpoints.Heartbeat));

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact(DisplayName = "GET /api/v1/health")]
        public async Task GetDeployInfo()
        {
            // Arrange
            var httpClient = new HttpClientProxy(CreateClient());
        
            // Act
            var response = await httpClient.GetAsync<V1.DeployInfoResponse>(Endpoints.DeployInfo);

            // Assert
            response.Should().NotBeNull();
        }

        [Fact(DisplayName = "GET /api/v1/health/checks")]
        public async Task CheckHealth()
        {
            // Arrange
            var httpClient = new HttpClientProxy(CreateClient());

            // Act
            var httpResponse = await httpClient.GetAsync(Endpoints.HealthChecks);

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeTrue();
        }
    }
}