using Bogus;
using Domain;
using FluentAssertions;
using IntegrationTests;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using V1 = WebApi.V1;

namespace TokenTests
{
    public class UserTokenTests : IntegrationTestBase
    {
        [Fact(DisplayName = "POST /api/v2/token (with valid credentials)")]
        public async Task CreateAccessToken_WithValidCredentials()
        {
            // Act
            var token = await TokenFactory.GetAccessTokenByUserCredentials(Constants.Username, Constants.UserPassword);

            // Assert
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact(DisplayName = "POST /api/v2/token (with invalid username)")]
        public async Task CreateAccessToken_WithInvalidKey()
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>();
            var httpClient = new Infrastructure.HttpClientProxy(factory.CreateClient());

            var request = new V1.TokenRequest()
            {
                Username = new Faker().Random.String2(UserConstants.UsernameMinLength, UserConstants.UsernameMaxLength),
                Password = Constants.UserPassword
            };

            // Act
            var httpResponse = await httpClient.PostAsync(request, "api/v2/token");

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact(DisplayName = "POST /api/v2/token (with invalid password)")]
        public async Task CreateAccessToken_WithInvalidSecret()
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>();
            var httpClient = new Infrastructure.HttpClientProxy(factory.CreateClient());

            var request = new V1.TokenRequest()
            {
                Username = Constants.Username,
                Password = new Faker().Internet.Password()
            };

            // Act
            var httpResponse = await httpClient.PostAsync(request, "api/v2/token");

            // Assert
            httpResponse.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact(DisplayName = "POST /api/v1/token/status (valid access token)")]
        public async Task Validate_ValidAccessToken()
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>();
            var httpClient = new Infrastructure.HttpClientProxy(factory.CreateClient());

            var token = await TokenFactory.GetAccessTokenByUserCredentials(Constants.Username, Constants.UserPassword);
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

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Shared.AuthorizationSchema.Bearer.ToString(), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyYTQ3YTRmYy0zZDkwLTRkZGItYTFlYy1hNjY0YzBhOGEyZjMiLCJ1c2VybmFtZSI6Iml2YW4uaXZhbm92IiwidXNlclJvbGUiOiJBZG1pbmlzdHJhdG9yIiwidXNlclN0YXR1cyI6IkFjdGl2ZSIsIm5iZiI6MTc3ODA5MDk1MywiZXhwIjoxNzc4MDkyMTUzLCJpYXQiOjE3NzgwOTA5NTMsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzAxIiwiYXVkIjoiQWxla3NhbmRyaW5hIFJhemhlbmtvdmEifQ.EKDIqIkGZkj5MKyr-6wsykPxs-s-Pl8zSAw7rDFrvG0");

            // Act
            var response = await httpClient.PostAsync<V1.TokenValidationResultResponse>("api/v1/token/status");

            // Assert
            response.Should().NotBeNull();
            response.IsValid.Should().BeFalse();
            response.Exception.Should().NotBeNull();
        }
    }
}