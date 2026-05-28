using Application;
using FluentAssertions;
using IntegrationTests;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Text;
using V1 = WebApi.V1;

namespace TokenTests
{
    public class ClientTokenTests : IntegrationTestBase
    {
        [Fact(DisplayName = "POST /api/v1/token (with valid credentials)")]
        public async Task CreateAccessToken_WithValidCredentials()
        {
            // Act
            var token = await TokenFactory.GetAccessTokenByClientCredentials(Constants.ClientKey, Constants.ClientSecret);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "POST /api/v1/token (with invalid key)")]
        public async Task CreateAccessToken_WithInvalidKey()
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>();
            var httpClient = new Infrastructure.HttpClientProxy(factory.CreateClient());

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientKey.Create()}:{Constants.ClientSecret}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Shared.AuthorizationSchema.Basic.ToString(), credentials);

            // Act
            var httpResponse = await httpClient.PostAsync("api/v1/token");

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact(DisplayName = "POST /api/v1/token (with invalid secret)")]
        public async Task CreateAccessToken_WithInvalidSecret()
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>();
            var httpClient = new Infrastructure.HttpClientProxy(factory.CreateClient());

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Constants.ClientKey}:{ClientSecret.Create()}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Shared.AuthorizationSchema.Basic.ToString(), credentials);

            // Act
            var httpResponse = await httpClient.PostAsync("api/v1/token");

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact(DisplayName = "POST /api/v1/token/status (valid access token)")]
        public async Task Validate_ValidAccessToken()
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>();
            var httpClient = new Infrastructure.HttpClientProxy(factory.CreateClient());

            var token = await TokenFactory.GetAccessTokenByClientCredentials(Constants.ClientKey, Constants.ClientSecret);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Shared.AuthorizationSchema.Bearer.ToString(), token);

            // Act
            var response = await httpClient.PostAsync<V1.TokenValidationResultResponse>("api/v1/token/status");

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeTrue();
            response.Exception.Should().BeNull();
        }

        [Fact(DisplayName = "POST /api/v1/token/status (invalid access token)")]
        public async Task Validate_InvalidAccessToken()
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>();
            var httpClient = new Infrastructure.HttpClientProxy(factory.CreateClient());

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Shared.AuthorizationSchema.Bearer.ToString(), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjbGllbnRJZCI6ImRiYTFkMjVhLTAwNjItNDllNy1iNGYwLTMxMjI0YTY5ZjllNCIsImNsaWVudFN0YXR1cyI6IkFDVElWRSIsImlzSW50ZXJuYWxDbGllbnQiOnRydWUsImNhbk5vdGlmeSI6dHJ1ZSwibmJmIjoxNzc4MTYwNjg4LCJleHAiOjE3NzgxNjE4ODgsImlhdCI6MTc3ODE2MDY4OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMDEiLCJhdWQiOiJBbGVrc2FuZHJpbmEgUmF6aGVua292YSJ9._N_-fZJke50_z137XRpZRXw3qhbKjEvbu22aapMIPB0");

            // Act
            var response = await httpClient.PostAsync<V1.TokenValidationResultResponse>("api/v1/token/status");

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeFalse();
            response.Exception.Should().NotBeNull();
        }
    }
}