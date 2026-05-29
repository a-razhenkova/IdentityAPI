using Application;
using FluentAssertions;
using IntegrationTests;
using Shared;
using System.Net.Http.Headers;
using System.Text;
using V1 = WebApi.V1;

namespace TokenTests
{
    public class ClientTokenTests : IntegrationTestBase
    {
        public ClientTokenTests(TestFactory factory) : base(factory) { }

        [Fact(DisplayName = "POST /api/v1/token (with valid credentials)")]
        public async Task CreateAccessToken_WithValidCredentials()
        {
            // Act
            var token = await TokenFactory.GetAccessTokenByClientCredentials(TestData.ClientKey, TestData.ClientSecret);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "POST /api/v1/token (with invalid key)")]
        public async Task CreateAccessToken_WithInvalidKey()
        {
            // Arrange
            var httpClient = new Infrastructure.HttpClientProxy(CreateClient());

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientKey.Create()}:{TestData.ClientSecret}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Basic.ToString(), credentials);

            // Act
            var httpResponse = await httpClient.PostAsync(Endpoints.Token_V1);

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact(DisplayName = "POST /api/v1/token (with invalid secret)")]
        public async Task CreateAccessToken_WithInvalidSecret()
        {
            // Arrange
            var httpClient = new Infrastructure.HttpClientProxy(CreateClient());

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{TestData.ClientKey}:{ClientSecret.Create()}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Basic.ToString(), credentials);

            // Act
            var httpResponse = await httpClient.PostAsync(Endpoints.Token_V1);

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact(DisplayName = "POST /api/v1/token/status (valid access token)")]
        public async Task Validate_ValidAccessToken()
        {
            // Arrange
            var httpClient = new Infrastructure.HttpClientProxy(CreateClient());

            var token = await TokenFactory.GetAccessTokenByClientCredentials(TestData.ClientKey, TestData.ClientSecret);
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

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Bearer.ToString(), TestData.InvalidClientToken);

            // Act
            var response = await httpClient.PostAsync<V1.TokenValidationResultResponse>(Endpoints.TokenStatus_V1);

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeFalse();
            response.Exception.Should().NotBeNull();
        }
    }
}