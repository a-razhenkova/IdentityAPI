using Microsoft.AspNetCore.Mvc.Testing;
using Shared;
using System.Net.Http.Headers;
using System.Text;
using V1 = WebApi.V1;

namespace IntegrationTests
{
    public static class TokenFactory
    {
        public async static Task<string> GetAccessTokenByClientCredentials(string key, string secret, CancellationToken cancellationToken = default)
        {
            var factory = new WebApplicationFactory<Program>();
            var httpClient = new Infrastructure.HttpClientProxy(factory.CreateClient());

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{secret}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Basic.ToString(), credentials);

            var response = await httpClient.PostAsync<V1.TokenResponse>("api/v1/token", cancellationToken)
                ?? throw new ArgumentNullException();

            return response.AccessToken;
        }

        public async static Task<string> GetAccessTokenByUserCredentials(string username, string password, CancellationToken cancellationToken = default)
        {
            var factory = new WebApplicationFactory<Program>();
            var httpClient = new Infrastructure.HttpClientProxy(factory.CreateClient());

            var request = new V1.TokenRequest()
            {
                Username = username,
                Password = password
            };

            var response = await httpClient.PostAsync<V1.TokenRequest, V1.TokenResponse>(request, "api/v2/token", cancellationToken)
                ?? throw new ArgumentNullException();

            return response.AccessToken;
        }
    }
}