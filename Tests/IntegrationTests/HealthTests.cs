using Microsoft.AspNetCore.Mvc.Testing;
using Shared;

namespace IntegrationTests
{
    public class HealthTests : IntegrationTestsBase
    {
        [Fact(DisplayName = "/api/v1/health/heartbeat")]
        public async Task HeartbeatAsync()
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>();
            var httpClient = factory.CreateClient();

            // Act
            var httpResponse = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, "/api/v1/health/heartbeat"));

            // Assert
            httpResponse.EnsureSuccessStatusCode();
        }

        //[Fact(DisplayName = "/api/v1/health")]
        //public async Task LoadDeployInfo()
        //{
        //    // Arrange
        //    var factory = new WebApplicationFactory<Program>();
        //    var httpClient = factory.CreateClient();
        //
        //    httpClient.DefaultRequestHeaders.Add(HttpHeaders.Authorization, "TODO");
        //
        //    // Act
        //    var httpResponse = await httpClient.GetAsync("/api/v1/health");
        //
        //    // Assert
        //    httpResponse.EnsureSuccessStatusCode();
        //}
    }
}