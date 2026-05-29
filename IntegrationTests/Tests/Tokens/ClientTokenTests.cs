using Application;
using FluentAssertions;
using IntegrationTests;
using Shared;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using V1 = WebApi.V1;

namespace TokenTests
{
    public class ClientTokenTests : IntegrationTestBase
    {
        public ClientTokenTests(TestFactory factory) : base(factory) { }

        [Fact(DisplayName = "POST /api/v1/token (with valid credentials)")]
        public async Task CreateAccessToken_WithValidCredentials_ReturnToken()
        {
            // Act
            var token = await TokenFactory.GetAccessTokenByClientCredentials(TestData.ClientKey, TestData.ClientSecret);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "POST /api/v1/token (with invalid key)")]
        public async Task CreateAccessToken_WithInvalidKey_Return401()
        {
            // Arrange
            var client = new Infrastructure.HttpClientProxy(_client);

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientKey.Create()}:{TestData.ClientSecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Basic.ToString(), credentials);

            // Act
            var response = await client.PostAsync(Endpoints.Token_V1);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/v1/token (with invalid secret)")]
        public async Task CreateAccessToken_WithInvalidSecret_Return401()
        {
            // Arrange
            var client = new Infrastructure.HttpClientProxy(_client);

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{TestData.ClientKey}:{ClientSecret.Create()}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Basic.ToString(), credentials);

            // Act
            var response = await client.PostAsync(Endpoints.Token_V1);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = "POST /api/v1/token (max wrong login attempts reached)")]
        public async Task CreateAccessToken_MaxWrongLoginAttemptsReached_Return403()
        {
            // Arrange
            var client = new Infrastructure.HttpClientProxy(_client);

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{TestData.ClientKey}:{ClientSecret.Create()}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Basic.ToString(), credentials);

            var response = await client.PostAsync(Endpoints.Token_V1); // called one more time internally
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            // Act
            response = await client.PostAsync(Endpoints.Token_V1); // third and fourth call

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            // setting the correct credentials
            credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{TestData.ClientKey}:{TestData.ClientSecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Basic.ToString(), credentials);

            response = await client.PostAsync(Endpoints.Token_V1);
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = "POST /api/v1/token/status (valid access token)")]
        public async Task Validate_ValidAccessToken_ReturnIsValidTrue()
        {
            // Arrange
            var client = new Infrastructure.HttpClientProxy(_client);

            var token = await TokenFactory.GetAccessTokenByClientCredentials(TestData.ClientKey, TestData.ClientSecret);
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

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Bearer.ToString(), TestData.InvalidClientToken);

            // Act
            var response = await client.PostAsync<V1.TokenValidationResultResponse>(Endpoints.TokenStatus_V1);

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeFalse();
            response.Exception.Should().NotBeNull();
        }
    }
}