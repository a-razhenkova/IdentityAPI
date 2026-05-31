using Application;
using FluentAssertions;
using IntegrationTests;
using System.Net;
using V1 = WebApi.V1;

namespace TokenTests
{
    public class ClientTokenTests : IntegrationTestBase
    {
        public ClientTokenTests(TestFactory factory) : base(factory) { }

        [Fact(DisplayName = "POST /api/v1/token (with valid credentials)")]
        public async Task CreateAccessToken_WithValidCredentials_ReturnToken()
        {
            // Arrange
            _client.SetBasicAuthorization(TestData.ClientKey, TestData.ClientSecret);

            // Act
            var response = await _client.PostAsync<V1.TokenResponse>(Endpoints.Token_V1);

            // Assert

            response.Should().NotBeNull();
            response.AccessToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "POST /api/v1/token (with invalid key)")]
        public async Task CreateAccessToken_WithInvalidKey_Return401()
        {
            // Arrange
            _client.SetBasicAuthorization(ClientKey.Create(), TestData.ClientSecret);

            // Act
            var response = await _client.PostAsync(Endpoints.Token_V1);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/v1/token (with invalid secret)")]
        public async Task CreateAccessToken_WithInvalidSecret_Return401()
        {
            // Arrange
            _client.SetBasicAuthorization(TestData.ClientKey, ClientSecret.Create());

            // Act
            var response = await _client.PostAsync(Endpoints.Token_V1);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/v1/token (max wrong login attempts reached)")]
        public async Task CreateAccessToken_MaxWrongLoginAttemptsReached_Return403()
        {
            // Arrange
            HttpResponseMessage response;

            _client.SetBasicAuthorization(TestData.ClientKey, ClientSecret.Create());

            for (int index = 0; index < 3; index++)
            {
                response = await _client.PostAsync(Endpoints.Token_V1);
                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }

            // Act
            response = await _client.PostAsync(Endpoints.Token_V1);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            // Assert
            _client.SetBasicAuthorization(TestData.ClientKey, TestData.ClientSecret);

            response = await _client.PostAsync(Endpoints.Token_V1);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "POST /api/v1/token/status (valid access token)")]
        public async Task Validate_ValidAccessToken_ReturnIsValidTrue()
        {
            // Arrange
            await SetAccessTokenByClientCredentialsAsync();

            // Act
            var response = await _client.PostAsync<V1.TokenValidationResultResponse>(Endpoints.TokenStatus_V1);

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeTrue();
            response.Exception.Should().BeNull();
        }

        [Fact(DisplayName = "POST /api/v1/token/status (invalid access token)")]
        public async Task Validate_InvalidAccessToken_ReturnIsValidFalse()
        {
            // Arrange
            _client.SetBearerAuthorization(TestData.InvalidClientToken);

            // Act
            var response = await _client.PostAsync<V1.TokenValidationResultResponse>(Endpoints.TokenStatus_V1);

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeFalse();
            response.Exception.Should().NotBeNull();
        }
    }
}