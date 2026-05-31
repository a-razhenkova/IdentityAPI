using Bogus;
using Domain;
using FluentAssertions;
using IntegrationTests;
using System.Net;
using V1 = WebApi.V1;

namespace TokenTests
{
    public class UserTokenTests : IntegrationTestBase
    {
        public UserTokenTests(TestFactory factory) : base(factory) { }

        [Fact(DisplayName = "POST /api/v2/token (with valid credentials)")]
        public async Task CreateAccessToken_WithValidCredentials_ReturnToken()
        {
            // Arrange
            var request = new V1.TokenRequest()
            {
                Username = TestData.Username,
                Password = TestData.UserPassword
            };

            // Act
            var response = await _client.PostAsync<V1.TokenRequest, V1.TokenResponse>(request, Endpoints.Token_V2);

            // Assert
            response.Should().NotBeNull();
            response.AccessToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "POST /api/v2/token (with invalid username)")]
        public async Task CreateAccessToken_WithInvalidKey_Return401()
        {
            // Arrange
            var request = new V1.TokenRequest()
            {
                Username = new Faker().Random.String2(UserConstants.UsernameMinLength, UserConstants.UsernameMaxLength),
                Password = TestData.UserPassword
            };

            // Act
            var response = await _client.PostAsync(request, Endpoints.Token_V2);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/v2/token (with invalid password)")]
        public async Task CreateAccessToken_WithInvalidSecret_Return401()
        {
            // Arrange
            var request = new V1.TokenRequest()
            {
                Username = TestData.Username,
                Password = new Faker().Internet.Password()
            };

            // Act
            var response = await _client.PostAsync(request, Endpoints.Token_V2);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/v2/token (wrong login attempts)")]
        public async Task CreateAccessToken_WrongLoginAttempts_Return401()
        {
            // Arrange
            HttpResponseMessage response;

            var request = new V1.TokenRequest()
            {
                Username = TestData.Username,
                Password = new Faker().Internet.Password()
            };

            // Act
            for (int index = 0; index < 3; index++)
            {
                response = await _client.PostAsync(request, Endpoints.Token_V2);
                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }

            response = await _client.PostAsync(request, Endpoints.Token_V2);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            // Assert
            request.Password = TestData.UserPassword;

            response = await _client.PostAsync(request, Endpoints.Token_V2);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "POST /api/v1/token/status (valid access token)")]
        public async Task Validate_ValidAccessToken_ReturnIsValidTrue()
        {
            // Arrange
            await SetAccessTokenByUserCredentialsAsync();

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
            _client.SetBearerAuthorization(TestData.InvalidUserToken);

            // Act
            var response = await _client.PostAsync<V1.TokenValidationResultResponse>(Endpoints.TokenStatus_V1);

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeFalse();
            response.Exception.Should().NotBeNull();
        }
    }
}