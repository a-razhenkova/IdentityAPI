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
                        string accessToken = await TokenFactory.GetAccessTokenByClientCredentials(Constants.ClientKey, Constants.ClientSecret, cancellationToken);
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationSchema.Bearer.ToString(), accessToken);
                    }
                    break;
            }
        }
    }
}