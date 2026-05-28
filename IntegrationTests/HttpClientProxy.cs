using Microsoft.AspNetCore.Mvc.Testing;
using Shared;
using System.Net.Http.Headers;
using System.Text;
using V1 = WebApi.V1;

namespace IntegrationTests
{
    public enum AuthType
    {
        ClientCredentials,
        UserCredentials
    }

    public class HttpClientProxy : Infrastructure.HttpClientProxy
    {
        private readonly AuthType _authType;

        public HttpClientProxy(HttpClient httpClient, AuthType authType = AuthType.ClientCredentials) : base(httpClient)
        {
            _authType = authType;
        }

        protected override async Task SetHttpClientAuthorizationAsync(CancellationToken cancellationToken = default)
        {
            switch (_authType)
            {
                case AuthType.ClientCredentials:
                    {
                        string accessToken = await GetAccessTokenByClientCredentials(cancellationToken);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Bearer.ToString(), accessToken);
                    }
                    break;
            }
        }

        private async static Task<string> GetAccessTokenByClientCredentials(CancellationToken cancellationToken = default)
        {
            const string path = "api/v1/token";
            const string key = "dba1d25a-0062-49e7-b4f0-31224a69f9e4";
            const string secret = "818fec5e-bff4-4396-85a7-9cc2eccd166f";

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{secret}"));

            var factory = new WebApplicationFactory<Program>();
            var httpClient = new Infrastructure.HttpClientProxy(factory.CreateClient());

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Basic.ToString(), credentials);

            var response = await httpClient.PostAsync<V1.TokenResponse>(path, cancellationToken)
                ?? throw new ArgumentNullException();

            return response.AccessToken;
        }
    }
}