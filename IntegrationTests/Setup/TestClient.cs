using Infrastructure;
using Shared;
using System.Net.Http.Headers;
using System.Text;

namespace IntegrationTests
{
    public sealed class TestClient : HttpClientProxy
    {
        public TestClient(HttpClient httpClient) : base(httpClient)
        {
            _isRetryWhenUnauthorizedAllowed = false;
        }

        public TestClient SetBasicAuthorization(string key, string secret)
        {
            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{secret}"));
            return SetAuthorization(AuthorizationSchema.Basic, credentials);
        }

        public TestClient SetBearerAuthorization(string token)
        {
            return SetAuthorization(AuthorizationSchema.Bearer, token);
        }

        public TestClient SetAuthorization(AuthorizationSchema schema, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(schema.ToString(), value);

            return this;
        }
    }
}