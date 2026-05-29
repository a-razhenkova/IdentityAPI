using Bogus;
using Domain;
using FluentAssertions;
using IntegrationTests;
using Shared;
using System.Net;
using System.Net.Http.Headers;
using V1 = WebApi.V1;

namespace TokenTests
{
    public class UserTokenTests : IntegrationTestBase
    {
        public UserTokenTests(TestFactory factory) : base(factory) { }

        [Fact(DisplayName = "POST /api/v2/token (with valid credentials)")]
        public async Task CreateAccessToken_WithValidCredentials_ReturnToken()
        {
            // Act
            var token = await TokenFactory.GetAccessTokenByUserCredentials(TestData.Username, TestData.UserPassword);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "POST /api/v2/token (with invalid username)")]
        public async Task CreateAccessToken_WithInvalidKey_Return401()
        {
            // Arrange
            var client = new Infrastructure.HttpClientProxy(_client);

            var request = new V1.TokenRequest()
            {
                Username = new Faker().Random.String2(UserConstants.UsernameMinLength, UserConstants.UsernameMaxLength),
                Password = TestData.UserPassword
            };

            // Act
            var response = await client.PostAsync(request, Endpoints.Token_V2);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/v2/token (with invalid password)")]
        public async Task CreateAccessToken_WithInvalidSecret_Return401()
        {
            // Arrange
            var client = new Infrastructure.HttpClientProxy(_client);

            var request = new V1.TokenRequest()
            {
                Username = TestData.Username,
                Password = new Faker().Internet.Password()
            };

            // Act
            var response = await client.PostAsync(request, Endpoints.Token_V2);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/v2/token (wrong login attempts)")]
        public async Task CreateAccessToken_WrongLoginAttempts_Return401()
        {
            // Arrange
            HttpResponseMessage response;
            var client = new Infrastructure.HttpClientProxy(_client);

            var request = new V1.TokenRequest()
            {
                Username = TestData.Username,
                Password = new Faker().Internet.Password()
            };

            response = await client.PostAsync(request, Endpoints.Token_V2); // called one more time internally
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            // Act
            response = await client.PostAsync(request, Endpoints.Token_V2); // third call and fourth call

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            // setting the correct credentials
            request.Password = TestData.UserPassword;

            response = await client.PostAsync(request, Endpoints.Token_V2);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "POST /api/v1/token/status (valid access token)")]
        public async Task Validate_ValidAccessToken_ReturnIsValidTrue()
        {
            // Arrange
            var client = new Infrastructure.HttpClientProxy(_client);

            var token = await TokenFactory.GetAccessTokenByUserCredentials(TestData.Username, TestData.UserPassword);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Bearer.ToString(), token);

            // Act
            var response = await client.PostAsync<V1.TokenValidationResultResponse>(Endpoints.TokenStatus_V1);

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeTrue();
            response.Exception.Should().BeNull();
        }

        [Fact(DisplayName = "POST /api/v1/token/status (invalid access token)")]
        public async Task Validate_InvalidAccessToken_ReturnIsValidFalse()
        {
            // Arrange
            var client = new Infrastructure.HttpClientProxy(_client);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Bearer.ToString(), TestData.InvalidUserToken);

            // Act
            var response = await client.PostAsync<V1.TokenValidationResultResponse>(Endpoints.TokenStatus_V1);

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeFalse();
            response.Exception.Should().NotBeNull();
        }
    }
}