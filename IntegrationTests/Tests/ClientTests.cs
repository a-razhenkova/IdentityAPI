using FluentAssertions;
using IntegrationTests;
using V1 = WebApi.V1;

namespace ClientTests
{
    public class ClientTests : IntegrationTestBase
    {
        public ClientTests(TestFactory factory) : base(factory) { }

        [Fact(DisplayName = "POST /api/v1/clients")]
        public async Task CreateClient()
        {
            // Arrange
            await SetAccessTokenByUserCredentialsAsync();

            var request = new CreateClientRequestFaker_V1().Generate();

            // Act
            var response = await _client.PostAsync<V1.CreateClientRequest, V1.SimpleResponse<string>>(request, Endpoints.Clients_V1);

            // Assert
            response.Should().NotBeNull();
            response.Value.Should().NotBeNullOrWhiteSpace();
        }
    }
}