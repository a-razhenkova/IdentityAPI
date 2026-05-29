using Shared;
using System.Net.Http.Headers;

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

        public HttpClientProxy(HttpClient httpClient, AuthType authType = AuthType.UserCredentials) : base(httpClient)
        {
            _authType = authType;
        }

        protected override async Task SetHttpClientAuthorizationAsync(CancellationToken cancellationToken = default)
        {
            switch (_authType)
            {
                case AuthType.ClientCredentials:
                    {
                        string accessToken = await TokenFactory.GetAccessTokenByClientCredentials(TestData.ClientKey, TestData.ClientSecret, cancellationToken);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Bearer.ToString(), accessToken);
                    }
                    break;
                case AuthType.UserCredentials:
                    {
                        string accessToken = await TokenFactory.GetAccessTokenByUserCredentials(TestData.Username, TestData.UserPassword, cancellationToken);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Bearer.ToString(), accessToken);
                    }
                    break;
            }
        }
    }
}