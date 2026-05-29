using Bogus;
using Domain;
using FluentAssertions;
using IntegrationTests;
using Shared;
using System.Net.Http.Headers;
using V1 = WebApi.V1;

namespace TokenTests
{
    public class UserTokenTests : IntegrationTestBase
    {
        public UserTokenTests(TestFactory factory) : base(factory) { }

        [Fact(DisplayName = "POST /api/v2/token (with valid credentials)")]
        public async Task CreateAccessToken_WithValidCredentials()
        {
            // Act
            var token = await TokenFactory.GetAccessTokenByUserCredentials(TestData.Username, TestData.UserPassword);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "POST /api/v2/token (with invalid username)")]
        public async Task CreateAccessToken_WithInvalidKey()
        {
            // Arrange
            var httpClient = new Infrastructure.HttpClientProxy(CreateClient());

            var request = new V1.TokenRequest()
            {
                Username = new Faker().Random.String2(UserConstants.UsernameMinLength, UserConstants.UsernameMaxLength),
                Password = TestData.UserPassword
            };

            // Act
            var httpResponse = await httpClient.PostAsync(request, Endpoints.Token_V2);

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact(DisplayName = "POST /api/v2/token (with invalid password)")]
        public async Task CreateAccessToken_WithInvalidSecret()
        {
            // Arrange
            var httpClient = new Infrastructure.HttpClientProxy(CreateClient());

            var request = new V1.TokenRequest()
            {
                Username = TestData.Username,
                Password = new Faker().Internet.Password()
            };

            // Act
            var httpResponse = await httpClient.PostAsync(request, Endpoints.Token_V2);

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact(DisplayName = "POST /api/v1/token/status (valid access token)")]
        public async Task Validate_ValidAccessToken()
        {
            // Arrange
            var httpClient = new Infrastructure.HttpClientProxy(CreateClient());

            var token = await TokenFactory.GetAccessTokenByUserCredentials(TestData.Username, TestData.UserPassword);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Bearer.ToString(), token);

            // Act
            var response = await httpClient.PostAsync<V1.TokenValidationResultResponse>(Endpoints.TokenStatus_V1);

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeTrue();
            response.Exception.Should().BeNull();
        }

        [Fact(DisplayName = "POST /api/v1/token/status (invalid access token)")]
        public async Task Validate_InvalidAccessToken()
        {
            // Arrange
            var httpClient = new Infrastructure.HttpClientProxy(CreateClient());

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Bearer.ToString(), TestData.InvalidUserToken);

            // Act
            var response = await httpClient.PostAsync<V1.TokenValidationResultResponse>(Endpoints.TokenStatus_V1);

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeFalse();
            response.Exception.Should().NotBeNull();
        }
    }
}